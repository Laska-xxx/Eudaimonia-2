using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
// 2. Renderer Feature для Unity 6 Render Graph
public class PS1PostProcessFeature : ScriptableRendererFeature
{
    [SerializeField] private Shader pixelationShader;
    [SerializeField] private Shader ditheringShader;
    [SerializeField] private Shader interlacingShader;

    private Material _pixelMat;
    private Material _ditherMat;
    private Material _interlaceMat;
    private PS1RenderPass _renderPass;

    private RTHandle _historyBuffer;

    public override void Create()
    {
        if (pixelationShader != null) _pixelMat = new Material(pixelationShader);
        if (ditheringShader != null) _ditherMat = new Material(ditheringShader);
        if (interlacingShader != null) _interlaceMat = new Material(interlacingShader);

        // Инициализируем пасс БЕЗ жестко заданного буфера
        _renderPass = new PS1RenderPass(_pixelMat, _ditherMat, _interlaceMat);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Пропускаем Scene View, чтобы не было конфликтов истории
        if (renderingData.cameraData.cameraType == CameraType.Game)
        {
            // Убеждаемся, что применяем только к финальной камере в стеке (Camera Stacking)
            bool isLastCameraInStack = renderingData.cameraData.resolveFinalTarget;
            if (isLastCameraInStack)
            {
                renderer.EnqueuePass(_renderPass);
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(_pixelMat);
        CoreUtils.Destroy(_ditherMat);
        CoreUtils.Destroy(_interlaceMat);

        // Очищаем историю при выключении фичи
        _renderPass?.Dispose();
    }

    private class PS1RenderPass : ScriptableRenderPass
    {
        private Material _pixelMat;
        private Material _ditherMat;
        private Material _interlaceMat;

        // Перенесли управление буфером истории внутрь самого пасса
        private RTHandle _historyBuffer;

        public PS1RenderPass(Material pixel, Material dither, Material interlace)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
            _pixelMat = pixel;
            _ditherMat = dither;
            _interlaceMat = interlace;
        }

        public void Dispose()
        {
            _historyBuffer?.Release();
        }

        private class PassData
        {
            public TextureHandle source;
            public TextureHandle history;
            public Material material;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var stack = VolumeManager.instance.stack;
            var customVolume = stack.GetComponent<PS1VolumeComponent>();

            if (customVolume == null || !customVolume.IsActive()) return;
            if (_pixelMat == null || _ditherMat == null || _interlaceMat == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();

            TextureHandle cameraColor = resourceData.activeColorTexture;
            if (!cameraColor.IsValid()) return;

            // 1. Обновляем параметры материалов
            _pixelMat.SetFloat("_PixelationFactor", customVolume.pixelationFactor.value);
            _ditherMat.SetFloat("_DitheringScale", customVolume.ditheringScale.value);
            _interlaceMat.SetFloat("_InterlacingSize", customVolume.interlacingSize.value);
            _interlaceMat.SetFloat("_InterlacedFrameIndex", Time.frameCount % 2);

            // 2. Создаем временные текстуры (target1, target2)
            TextureDesc desc = renderGraph.GetTextureDesc(cameraColor);
            desc.name = "PS1_IntermediateTarget1";
            desc.clearBuffer = false;
            desc.depthBufferBits = 0;
            desc.msaaSamples = MSAASamples.None;

            TextureHandle target1 = renderGraph.CreateTexture(desc);
            desc.name = "PS1_IntermediateTarget2";
            TextureHandle target2 = renderGraph.CreateTexture(desc);

            // === КРИТИЧЕСКОЕ ИСПРАВЛЕНИЕ ===
            // Динамически выделяем и обновляем буфер истории, чтобы он ИДЕАЛЬНО совпадал
            // с текущим разрешением и форматом кадра (например, если включен HDR)
            RenderTextureDescriptor historyDesc = cameraData.cameraTargetDescriptor;
            historyDesc.depthBufferBits = 0;
            historyDesc.msaaSamples = 1; // Убираем сглаживание для буфера истории

            RenderingUtils.ReAllocateHandleIfNeeded(ref _historyBuffer, historyDesc, FilterMode.Point, TextureWrapMode.Clamp, name: "PS1_HistoryBuffer");
            TextureHandle historyHandle = renderGraph.ImportTexture(_historyBuffer);

            // === ПАСС 1: Пикселизация (Камера -> Target 1) ===
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("PS1 Pixelation", out var passData))
            {
                passData.source = cameraColor;
                passData.material = _pixelMat;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(target1, 0, AccessFlags.Write);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // === ПАСС 2: Дизеринг (Target 1 -> Target 2) ===
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("PS1 Dithering", out var passData))
            {
                passData.source = target1;
                passData.material = _ditherMat;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(target2, 0, AccessFlags.Write);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // === ПАСС 3: Чересстрочность (Target 2 -> Камера) ===
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("PS1 Interlacing", out var passData))
            {
                passData.source = target2;
                passData.history = historyHandle;
                passData.material = customVolume.enableInterlacing.value ? _interlaceMat : null;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.UseTexture(passData.history, AccessFlags.Read);
                builder.SetRenderAttachment(cameraColor, 0, AccessFlags.Write);

                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                    if (data.material != null)
                    {
                        context.cmd.SetGlobalTexture("_PreviousFrame", data.history);
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                    }
                    else
                    {
                        Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
                    }
                });
            }

            // === ПАСС 4: Обновление Истории (Камера -> History Buffer) ===
            using (var builder = renderGraph.AddRasterRenderPass<PassData>("PS1 Update History", out var passData))
            {
                passData.source = cameraColor;

                builder.UseTexture(passData.source, AccessFlags.Read);
                builder.SetRenderAttachment(historyHandle, 0, AccessFlags.Write);
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                    Blitter.BlitTexture(context.cmd, data.source, new Vector4(1, 1, 0, 0), 0, false);
                });
            }
        }
    }
}