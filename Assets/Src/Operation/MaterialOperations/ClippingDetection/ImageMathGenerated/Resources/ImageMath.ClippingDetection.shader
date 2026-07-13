Shader "ImageMath/ClippingDetection"{
    Properties {}
    SubShader {
        Cull Off ZWrite Off ZTest Always
        ColorMask[ImageMath_ChannelMask]

        CGINCLUDE


        
        #include "UnityCG.cginc"

        #define Pi 3.1415926535897932384626433832795
        #define SquareRootOf2 1.4142135623730950488016887242097
        #define Epsilon 10e-6



        struct VSI {
            float4 position : POSITION;
            float2 uv : TEXCOORD0;
        };
        
        struct VSO {
            float4 position : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        float4 ImageMath_RenderTargetSize;
        #define RenderTargetSize ImageMath_RenderTargetSize.xy
        #define InverseRenderTargetSize ImageMath_RenderTargetSize.zw

        Texture2D<float4> Texture;
        SamplerState samplerTexture;

        VSO vert(VSI input) {
            VSO result;
            result.position = input.position;
        	result.position = UnityObjectToClipPos(result.position);
            result.uv = input.uv;
            return result;
        }
        
        float4 frag(VSO input) : SV_Target {
            int3 pixelPosition = int3(input.position.xy, 0);
            uint2 textureSize = 0;
            uint levels = 0;
            Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
            /*int minX = max(0, pixelPosition.x - 1);
            int maxX = min(int(textureSize.x) - 1, pixelPosition.x + 1);
            int minY = max(0, pixelPosition.y - 1);
            int maxY = min(int(textureSize.y) - 1, pixelPosition.y + 1);
            float3 referenceValue = Texture.Load(int3(minX, minY, 0)).rgb;*/
            float3 values[9];
            int index = 0;
            for (int x = -1; x <= 1; x++){
                for (int y = -1; y <= 1; y++){
                    int3 samplePosition = pixelPosition + int3(x, y, 0);
                    samplePosition = clamp(samplePosition, int3(0, 0, 0), int3(textureSize - 1, levels - 1));
                    values[index] = Texture.Load(samplePosition).rgb;
                    index++;
                }
            }
            bool3 clipping = bool3(true, true, true);
            for (int i = 1; i < 9; i++){
                bool3 equal = values[0] == values[i];
                clipping = clipping && equal;
            }
            return float4(clipping * values[0], 1.0);
        }

        ENDCG

        Pass {//0 AssignTo
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{//1 AddTo
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{//2 MultiplyTo
            Blend DstColor Zero
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{//3 AlphaBlendTo
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{//4 PremultipliedAlphaBlendTo
            Blend One OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{//5 
            Blend One One
            BlendOp Max
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{//6
            Blend One One
            BlendOp Min
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

    }
}