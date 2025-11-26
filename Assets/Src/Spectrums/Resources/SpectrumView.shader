Shader "ImageMath/Views/SpectrumView"
{
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos  : SV_POSITION;
            };

            float4 _Color;
            float _StartWavelength;
            float _NanometersPerUnit;
            float _YScale;

            v2f vert(appdata v) {
                v2f o;
                float3 pos = v.vertex.xyz;
                o.pos = UnityObjectToClipPos(float4(pos, 1));
                return o;
            }

            float4 frag(v2f i) : SV_Target {
                return _Color;
            }
            ENDCG
        }
    }
}