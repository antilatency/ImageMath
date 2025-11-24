Shader "ImageMath/Views/SpectrumView"
{
    Properties {
        
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos  : SV_POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            float4 _Color;

            float4 frag(v2f i) : SV_Target {
                return _Color;
            }
            ENDCG
        }
    }
}