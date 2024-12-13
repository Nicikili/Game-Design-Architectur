Shader "URP/Pencil Contour" {
   Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _NoiseTex ("Noise Tex", 2D) = "black" {}
        _ErrorPeriod ("Error Period", Float) = 25.0
        _ErrorRange ("Error Range", Float) = 0.0015
        _NoiseAmount ("Noise Amount", Float) = 0.02
        _EdgeOnly ("Edge Only", Float) = 1.0
        _EdgeColor ("Edge Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 1)
        _SampleDistance ("Sample Distance", Float) = 1.0
        _Sensitivity ("Sensitivity", Vector) = (1, 1, 1, 1)
    }
    SubShader {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        Pass {
            Name "PencilContourPass"
            Tags { "LightMode"="UniversalForward" }
            ZTest Always Cull Off ZWrite On Blend Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float4 _MainTex_TexelSize;
            float _ErrorPeriod;
            float _ErrorRange;
            float _NoiseAmount;
            half _EdgeOnly;
            half4 _EdgeColor;
            half4 _BackgroundColor;
            float _SampleDistance;
            float4 _Sensitivity;

            // Screen Space Depth/Normals
            TEXTURE2D(_CameraDepthTexture);
            SAMPLER(sampler_CameraDepthTexture);

            TEXTURE2D(_CameraNormalsTexture);
            SAMPLER(sampler_CameraNormalsTexture);

            struct VertexInput {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct FragmentInput {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            VertexInput vert(VertexInput v) {
                VertexInput o;
                o.position = TransformObjectToHClip(v.position.xyz);
                o.uv = v.uv;
                return o;
            }

            float CheckSame(float3 centerNormal, float centerDepth, float3 sampleNormal, float sampleDepth) {
                // Calculate differences
                float diffNormal = length(centerNormal - sampleNormal) * _Sensitivity.x;
                float diffDepth = abs(centerDepth - sampleDepth) * _Sensitivity.y;

                // Return edge factor
                return step(diffNormal + diffDepth, 0.1);
            }

            half4 frag(FragmentInput i) : SV_Target {
                float2 uv = i.uv;

                // Sample depth and normals
                float centerDepth = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv).r;
                float3 centerNormal = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv).rgb;

                float3 sampleNormal1 = SAMPLE_TEXTURE2D(_CameraNormalsTexture, sampler_CameraNormalsTexture, uv + float2(_MainTex_TexelSize.xy) * _SampleDistance).rgb;
                float sampleDepth1 = SAMPLE_TEXTURE2D(_CameraDepthTexture, sampler_CameraDepthTexture, uv + float2(_MainTex_TexelSize.xy) * _SampleDistance).r;

                float edge = CheckSame(centerNormal, centerDepth, sampleNormal1, sampleDepth1);

                // Add noise and apply edge color
                float noise = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, uv).r * _NoiseAmount;
                float finalEdge = edge * (1.0 - noise);

                // Blend edge with background
                half4 finalColor = lerp(_BackgroundColor, _EdgeColor, finalEdge);

                // Ensure fully opaque output
                return half4(finalColor.rgb, 1.0);
            }
            ENDHLSL
        }
    }
    FallBack Off
}