Shader "Custom/ProceduralLines"
{
    Properties {
        _MainTex ("Texture", 3D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always

            // --- Common directives ---
            CGPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #include "UnityCG.cginc"

            // --- Structs ---
            struct VSI {
                uint vertexID : SV_VertexID;
            };

            struct VSO {
                float4 pos : SV_POSITION;
                uint vertexID : VERTEX_ID;
            };

            struct GSO {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
            };

            float4 _Color;

            // --- Vertex shader ---
            VSO vert(VSI v) {
                VSO output = (VSO)0;
                output.vertexID = v.vertexID;
                return output;
            }

            float4x4 _ObjectToWorld;

            void AddVertex(inout LineStream<GSO> lineStream, float3 position, float3 color){
                GSO v;
                float3 worldPos = mul(_ObjectToWorld, float4(position, 1)).xyz;
                v.pos = UnityObjectToClipPos(float4(worldPos, 1));
                v.color = float4(color, 0.1);
                lineStream.Append(v);
            }

            float3 GetVertexPosition(uint3 id){
                return id / 31.0;
            }

            Texture3D<float4> _MainTex;

            [maxvertexcount(5)]
            void geom(point VSO input[1], inout LineStream<GSO> lineStream) {
                uint index = input[0].vertexID;
                uint3 id = uint3(
                    index % 32,
                    (index / 32) % 32,
                    index / (32 * 32)
                );

                float3 thisPosition = GetVertexPosition(id);
                float3 rightPosition = GetVertexPosition(id + uint3(1, 0, 0));
                float3 upPosition = GetVertexPosition(id + uint3(0, 1, 0));
                float3 forwardPosition = GetVertexPosition(id + uint3(0, 0, 1));

                bool3 last = id == 31;
                if (!last.x) AddVertex(lineStream, rightPosition, rightPosition);
                AddVertex(lineStream, thisPosition, thisPosition);                
                if (!last.y) AddVertex(lineStream, upPosition, upPosition);
                lineStream.RestartStrip();
                if (!last.z){
                    AddVertex(lineStream, thisPosition, thisPosition);
                    AddVertex(lineStream, forwardPosition, forwardPosition);
                }

                /*float3 tipPos = thisPosition + float3(0.01, 0.01, 0.01);

                GSO v1, v2;
                v1.pos = UnityObjectToClipPos(float4(basePos, 1));
                v2.pos = UnityObjectToClipPos(float4(tipPos, 1));

                v1.color = _Color;
                v2.color = 0;

                lineStream.Append(v1);
                lineStream.Append(v2);*/
            }

            // --- Fragment shader ---
            float4 frag(GSO i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
