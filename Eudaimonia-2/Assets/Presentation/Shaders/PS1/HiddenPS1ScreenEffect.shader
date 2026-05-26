Shader "Hidden/PS1ScreenEffect"
{
    Properties
    {
        _Resolution ("Pixelation Resolution", Float) = 320
        _ColorDepth ("Color Depth", Float) = 32
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100
        ZWrite Off Cull Off ZTest Always

        Pass
        {
            Name "PS1Pass"

            HLSLPROGRAM
            // В Unity 6 мы используем встроенный вертексный шейдер Vert из Blit.hlsl
            #pragma vertex Vert 
            #pragma fragment frag

            // Обязательные библиотеки для нового пайплайна
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _Resolution;
            float _ColorDepth;

            half4 frag (Varyings input) : SV_Target
            {
                // input.texcoord уже содержит правильные UV-координаты экрана от Blitter'а
                float2 uv = input.texcoord;
                
                // Пикселизация
                float aspect = _ScreenParams.x / _ScreenParams.y;
                uv.x = floor(uv.x * _Resolution) / _Resolution;
                uv.y = floor(uv.y * (_Resolution / aspect)) / (_Resolution / aspect);

                // Чтение текстуры через новый стандарт _BlitTexture
                half4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // Постеризация
                col.rgb = floor(col.rgb * _ColorDepth) / _ColorDepth;

                return col;
            }
            ENDHLSL
        }
    }
}