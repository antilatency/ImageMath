Shader "ImageMath/MaxOperation"{
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
            int2 M = RenderTargetSize;
            int2 N;
            Texture.GetDimensions(N.x, N.y);
            int2 outputIndex = input.position.xy;
            int2 baseBlockSize = N / M;
            int2 remainder = N % M;
            int2 startIndex = outputIndex * baseBlockSize + min(outputIndex, remainder);
            int2 blockSize = baseBlockSize + (outputIndex < remainder ? 1 : 0);
            int2 maxBlockSize = baseBlockSize + (remainder>0);
            int pixelsPerBlock = blockSize.x * blockSize.y;
            float invPixelsPerBlock = 1.0f / pixelsPerBlock;
            int maxPixelsPerBlock = maxBlockSize.x * maxBlockSize.y;
            float invMaxPixelsPerBlock = 1.0f / maxPixelsPerBlock;
            float4 a = Texture.Load(int3(startIndex, 0));

            for (int i = 1; i < blockSize.x; i++) {
                int2 index = startIndex + int2(i, 0);
                float4 b = Texture.Load(int3(index, 0));
                {
                    a = max(a, b);
                }
            }
            for (int i = 0; i < blockSize.x; i++) {
                for (int j = 1; j < blockSize.y; j++) {
                    int2 index = startIndex + int2(i, j);
                    float4 b = Texture.Load(int3(index, 0));
                    {
                        a = max(a, b);
                    }
                }
            }

            return a;
        }

        ENDCG

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{
            Blend One One
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{
            Blend DstColor Zero
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{
            Blend One One
            BlendOp Max
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

        Pass{
            Blend One One
            BlendOp Min
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag            
            ENDCG
        }

    }
}