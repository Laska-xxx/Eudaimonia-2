Shader "Hidden/PS1PostProcess"
{
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment frag
            #pragma target 3.5 

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            float _DitheringScale;

            // Вычисление смещения дизеринга
            float PSX_GetDitherOffset(int2 pixelPosition)
            {
                const float ditheringMatrix4x4[16] =
                {
                    -4.0,  0.0, -3.0,  1.0,
                     2.0, -2.0,  3.0, -1.0,
                    -3.0,  1.0, -4.0,  0.0,
                     3.0, -1.0,  2.0, -2.0
                };

                int x = abs(pixelPosition.x) % 4;
                int y = abs(pixelPosition.y) % 4;
                return ditheringMatrix4x4[x + y * 4];
            }

            // Функция дизеринга цвета
            half4 PSX_DitherColor(half4 color, int2 pixelPosition)
            {
                float4 col255 = (color * 255.0f) + PSX_GetDitherOffset(pixelPosition);
                col255 = max(col255, 0.0f); 

                float4 quantizedCol = floor(col255 / 8.0f);
                return quantizedCol / 31.0f;
            }

            half4 frag(Varyings input) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                
                // input.positionCS.xy содержит точные экранные координаты (позицию пикселя)
                return PSX_DitherColor(col, (int2)floor(input.positionCS.xy * _DitheringScale));
            }
            ENDHLSL
        }
    }
}