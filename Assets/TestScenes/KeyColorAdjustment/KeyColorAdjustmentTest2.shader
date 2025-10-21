Shader "Hidden/KeyColorAdjustmentTest2"
{
    Properties
    {
        _ChromasTexture ("Chromas Texture", 2D) = "white" {}
        _GlobalAxisX("Global Axis X", Vector) = (1, 0, 0)
        _GlobalAxisY("Global Axis Y", Vector) = (0, 1, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float3 _GlobalAxisX;
            float3 _GlobalAxisY;
            float4 _ChromaRect;

            Texture2D<float4> _ChromasTexture;

            float4 DrawCircle(float2 center, float radius, float thickness, float4 color, float4 inputColor, float2 uv){
                float dist = distance(uv, center);
                float offset = 1- min(1,abs(dist - radius) / thickness);

                return lerp(inputColor, color, offset);
            }

            float4 DrawCross(float2 center, float thickness, float4 color, float4 inputColor, float2 uv){
                float2 a = abs(uv - center);
                float dist = min(a.x, a.y);
                float offset = 1 - min(1, dist / thickness);

                return lerp(inputColor, color, offset);
            }
            float4 DrawVerticalLine(float center, float thickness, float4 color, float4 inputColor, float2 uv){
                float dist = abs(uv.x - center);
                float offset = 1 - min(1, dist / thickness);

                return lerp(inputColor, color, offset);
            }

            float2 GetChroma(int index){
                return _ChromasTexture.Load(int3(index, 0, 0)).xy;
            }

            float2 GetChromaLocal(int index){
                return (_ChromasTexture.Load(int3(index, 0, 0)).xy - _ChromaRect.xy) / _ChromaRect.zw;
            }


            bool InRect(float2 uv, float4 rect) {
                //xy - offset, zw - size
                return all(uv>= rect.xy && uv<= rect.xy + rect.zw);
            }


            fixed4 frag (v2f input) : SV_Target {
                static const float4 adjustedColorMarkerColor = 1;
                static const float4 averageColorMarkerColor = float4(0, 0, 0, 1);
                static const float4 markerColor = float4(0,0,0, 1);

                float4 color = float4(0, 0, 0, 1);

                if (input.uv.x>0.5){
                    float2 uv = input.uv;
                    uv.x = 2*(uv.x - 0.5);
                    uint2 ChromasAndColorsTextureSize;
                    _ChromasTexture.GetDimensions(ChromasAndColorsTextureSize.x, ChromasAndColorsTextureSize.y);

                    //use ddx(i.uv) and ddy(i.uv) to get uv to pixels scale
                    float scale = ddx(uv.x);

                    float2 localUv = uv*_ChromaRect.zw + _ChromaRect.xy;
                    color = float4(localUv.x * _GlobalAxisX + localUv.y * _GlobalAxisY , 1);

                    color = DrawCross(GetChromaLocal(0), 2*scale, adjustedColorMarkerColor, color, uv);
                    color = DrawCircle(GetChromaLocal(0), 0.04, 2*scale, adjustedColorMarkerColor, color, uv);
                    color = DrawCircle(GetChromaLocal(1), 0.03, 2*scale, averageColorMarkerColor, color, uv);

                    for (int i = 2; i < ChromasAndColorsTextureSize.x; i++)
                    {
                        float2 chroma = GetChromaLocal(i);
                        color = DrawCircle(chroma, 0.01, 2*scale, markerColor, color, uv);
                    }


                } else {
                    float2 uv = input.uv;
                    uv.x = 2*uv.x;
                    float scale = ddx(uv.x);

                    bool inRect = InRect(uv, _ChromaRect);

                    color = float4(uv.x * _GlobalAxisX + uv.y * _GlobalAxisY , 1);

                    color = lerp(color, float4(0, 0, 0, 1), inRect ? 0 : 0.25);
                    color = DrawCross(GetChroma(0), 2*scale, adjustedColorMarkerColor, color, uv);
                    //color = float4(0.1, 0.1, 0.1, 1);
                }

                color = DrawVerticalLine(0.5, 2*ddx(input.uv.x), float4(0, 0, 0, 1), color, input.uv);
                //toSRGB
                return color;

            }
            ENDCG
        }
    }
}
