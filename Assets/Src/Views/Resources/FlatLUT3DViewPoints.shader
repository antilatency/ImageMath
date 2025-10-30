Shader "ImageMath/Views/FlatLUT3DViewPoints"
{
    Properties {
        FlatLUT3D ("FlatLUT3D", 2D) = "white" {}
        Alpha ("Alpha", Float) = 1.0
        PointSize ("Point Size", Float) = 0.125
    }

    SubShader {
        Pass { //points
            Blend Off
            ZTest LEqual

            CGPROGRAM   
            #pragma target 4.0
            #pragma vertex vert
            #pragma geometry geomPoints
            #pragma fragment fragPoints
            #include "UnityCG.cginc"
            #include "FlatLUT3DView.cginc"

            struct VSI {
                float3 position : POSITION;
            };

            struct VSO {
                float4 position : SV_POSITION;
                uint3 index: INDEX;
            };

            struct GSO {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            VSO vert(VSI v) {
                VSO output = (VSO)0;
                output.position = float4(v.position, 1);
                output.index = uint3(v.position * (Size-1));
                return output;
            }

            float PointSize = 0.125f;

            void AddQuad(inout TriangleStream<GSO> stream, float3 position, float3 color, float size){
                GSO v;
                float3 worldPos = mul(unity_ObjectToWorld, float4(position, 1)).xyz;
                float3 toCamera = normalize(_WorldSpaceCameraPos - worldPos);
                float3 right = normalize(cross(toCamera, float3(0, 1, 0)));
                float3 up = cross(toCamera, right);



                //v.pos = UnityObjectToClipPos(float4(worldPos, 1));
                //v.color = float4(color, Alpha);
                float screenAspect = _ScreenParams.x / _ScreenParams.y;
                //float2 offset = float2(size / screenAspect, size);
                static const float2 quadOffsets[4] = {
                    float2(-1, -1),
                    float2(1, -1),
                    float2(-1, 1),
                    float2(1, 1)                
                };


                for (int i = 0; i < 4; ++i) {
                    float3 w = worldPos + (right * quadOffsets[i].x + up * quadOffsets[i].y) * size;
                    GSO vi = (GSO)0;
                    vi.pos = mul(UNITY_MATRIX_VP, float4(w, 1.0));
                    vi.color = float4(color, 1);
                    vi.uv = quadOffsets[i];
                    stream.Append(vi);
                }
            }

        
            [maxvertexcount(4)]
            void geomPoints(point VSO input[1], inout TriangleStream<GSO> stream) {
                uint2 textureSize;
                FlatLUT3D.GetDimensions(textureSize.x, textureSize.y);

                uint3 id = input[0].index;

                float3 thisPosition = input[0].position.xyz;
                float3 thisValue = GetValue(id, textureSize.x);            

                AddQuad(stream, thisValue, thisPosition, PointSize / Size);
                
            }

            
            float4 fragPoints(GSO i) : SV_Target {
                float r = length(i.uv);
                clip(1-r);
                return i.color;
            }


            ENDCG
        }
    }
}
