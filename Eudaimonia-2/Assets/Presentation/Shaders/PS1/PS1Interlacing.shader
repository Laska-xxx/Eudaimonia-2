Shader "Hidden/PS1Interlacing"
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

            // Правильное объявление дополнительной текстуры в URP
            TEXTURE2D(_PreviousFrame);
            SAMPLER(sampler_PreviousFrame);

            float _InterlacedFrameIndex;
            float _InterlacingSize;

            half4 frag(Varyings input) : SV_Target
            {
                // Текущий кадр
                half4 col = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, input.texcoord);
                // Предыдущий кадр из History Buffer
                half4 previousColor = SAMPLE_TEXTURE2D(_PreviousFrame, sampler_PreviousFrame, input.texcoord);

                int2 pixelPosition = (int2)input.positionCS.xy;
                float interlacingAreaCheck = floor(pixelPosition.y / _InterlacingSize) % 2 == round(_InterlacedFrameIndex);
                
                return lerp(col, previousColor, interlacingAreaCheck);
            }
            ENDHLSL
        }
    }
}