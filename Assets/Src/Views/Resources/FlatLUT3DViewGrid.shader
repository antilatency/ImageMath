Shader "ImageMath/Views/FlatLUT3DViewGrid"
{
    Properties {
        FlatLUT3D ("FlatLUT3D", 2D) = "white" {}
        Alpha ("Alpha", Float) = 1.0
        PointSize ("Point Size", Float) = 0.125
    }
    SubShader {
        
        Pass { //lines transparent
            Blend SrcAlpha OneMinusSrcAlpha
            ZTest LEqual
            ZWrite Off

            CGPROGRAM  
            #pragma target 4.0
            #pragma vertex vert
            #pragma fragment fragLines            
            #include "UnityCG.cginc"
            #include "FlatLUT3DView.cginc" 
            

            struct VSI {
                float3 position : POSITION;
            };

            struct VSO {
                float4 position : SV_POSITION;
                float4 color : COLOR;
            };

            struct GSO {
                float4 pos : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            float Alpha;

            VSO vert(VSI v) {
                VSO output = (VSO)0;
                uint3 index = uint3(v.position * (Size-1));

                uint2 textureSize;
                FlatLUT3D.GetDimensions(textureSize.x, textureSize.y);
                float3 value = GetValue(index, textureSize.x);                

                output.position = UnityObjectToClipPos(float4(value, 1));
                output.color = float4(v.position, Alpha);
                return output;
            }

            float4 fragLines(GSO i) : SV_Target {
                return i.color;
            }

            ENDCG
        }
    }
}
