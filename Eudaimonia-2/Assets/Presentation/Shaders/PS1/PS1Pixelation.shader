Shader "Hidden/PS1Pixelation"
{
    SubShader
    {
        // Отключаем отсечение и запись в буфер глубины для Post-Processing
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma target 3.5 

            // Подключаем ядро URP и утилиты для Blitter
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _PixelationFactor;

            half4 frag(Varyings input) : SV_Target
            {
                float2 screenResolution = _ScreenParams.xy;
                float2 pixelScalingFactor = screenResolution * _PixelationFactor;

                // input.texcoord берется из сгенерированной структуры Varyings
                float2 pixelOrigin = floor(input.texcoord * pixelScalingFactor) / pixelScalingFactor;

                // Читаем пиксели из автоматически прокинутой _BlitTexture
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, pixelOrigin);

                return col;
            }
            ENDHLSL
        }
    }
}