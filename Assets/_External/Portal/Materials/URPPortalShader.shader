Shader "Custom/URPPortalShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [LeftEyeTexture] _LeftEyeTex("Left Eye", 2D) = "white" {}
        [RightEyeTexture] _RightEyeTex("Right Eye", 2D) = "white" {}
        // [FallbackTexture] _FallbackMap("Fallback Map", 2D) = "White" {}
        // [Toggle] _UseFallback ("Use Fallback", Float) = 0 
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            //left eye
            TEXTURE2D(_LeftEyeTex);
            SAMPLER(sampler_LeftEyeTex);

            //right eye
            TEXTURE2D(_RightEyeTex);
            SAMPLER(sampler_RightEyeTex);

            // TEXTURE2D(_FallbackMap);
            // SAMPLER(sampler_FallbackMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
                // float _UseFallback;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                UNITY_SETUP_INSTANCE_ID(IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                OUT.screenPos = GetVertexPositionInputs(IN.positionOS.xyz).positionNDC;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // if (_UseFallback)
                // {
                //     half4 color = SAMPLE_TEXTURE2D(_FallbackMap, sampler_FallbackMap, IN.screenPos.xy) * _BaseColor;
                //     return color;
                // }

                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

                IN.screenPos /= IN.screenPos.w;
                float2 uv = IN.screenPos.xy;

                //sample both left and right but use lerp to show one or the other
                half4 left = SAMPLE_TEXTURE2D(_LeftEyeTex,  sampler_LeftEyeTex,  uv);
                half4 right = SAMPLE_TEXTURE2D(_RightEyeTex, sampler_RightEyeTex, uv);
                return lerp(left, right, unity_StereoEyeIndex) * _BaseColor;
            }
            ENDHLSL
        }
    }
}
