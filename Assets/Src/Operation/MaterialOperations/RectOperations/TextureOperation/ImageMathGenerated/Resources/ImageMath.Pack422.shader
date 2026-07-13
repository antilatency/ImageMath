Shader "ImageMath/Pack422"{
    Properties {}
    SubShader {
        Cull Off ZWrite Off ZTest Always
        ColorMask[ImageMath_ChannelMask]

        CGINCLUDE

        #pragma multi_compile_local Layout_Cb0Y0Cr0Y1
        
        #include "UnityCG.cginc"

        #define Pi 3.1415926535897932384626433832795
        #define SquareRootOf2 1.4142135623730950488016887242097
        #define Epsilon 10e-6

        uint2 outputDimensions(uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(w * 2, h);
        	#endif
        }
        uint2 sourcePosition(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(((x/4*2) + uint4(0,0,0,1))[x%4], y);
        	#endif
        }
        uint sourceComponent(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint4(1,0,2,0)[x%4];
        	#endif
        }

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
        float2 Position;
        float2 Size;

        VSO vert(VSI input) {
            VSO result;
            result.position = input.position;
            result.position.xy *= Size;
        	result.position.xy += Position;
        	result.position = UnityObjectToClipPos(result.position);
            result.uv = input.uv;
            return result;
        }
        
        float4 frag(VSO input) : SV_Target {
            //assuming target texture height is double source texture height
            uint2 textureSize = 0;
            uint levels = 0;
            Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
            uint2 outputSize = outputDimensions(textureSize.x, textureSize.y);
            int2 pixelPosition = input.position.xy;
            uint2 p = sourcePosition(pixelPosition.x, pixelPosition.y, textureSize.x, textureSize.y);
            uint component = sourceComponent(pixelPosition.x, pixelPosition.y, textureSize.x, textureSize.y);
            float4 value = Texture.Load(int3(p.x, p.y, 0));
            return value[component];
            /*uint2 halfTextureSize = textureSize / 2;
            uint2 pixelPosition = input.position.xy;
            #if Layout_Cb0Y0Cr0Y1
            int field = pixelPosition.x % 4;
            int clasterIndex = pixelPosition.x / 4;
            int leftPixelX = 2 * clasterIndex;
            int rightPixelX = 2 * clasterIndex + 1;
            switch (field) {
                case 0: {
                    float y0 = Texture.Load(int3(leftPixelX, pixelPosition.y, 0)).r;
                    return y0;
                }
                case 1: {
                    float u = Texture.Load(int3(leftPixelX, pixelPosition.y, 0)).g;
                    return u;
                }
                case 2: {
                    float y1 = Texture.Load(int3(rightPixelX, pixelPosition.y, 0)).r;
                    return y1;
                }
                case 3: {
                    float v = Texture.Load(int3(leftPixelX, pixelPosition.y, 0)).b;
                    return v;
                }
            }
            #endif*/
            /*if (pixelPosition.y >= textureSize.y){
                float r = Texture.Load(int3(pixelPosition.x, pixelPosition.y - textureSize.y, 0)).r;
                return r;
            }
            if (pixelPosition.x >= halfTextureSize.x) {
                    int x = 2 * (pixelPosition.x - halfTextureSize.x);
                    float b = Texture.Load(int3(x, pixelPosition.y, 0)).b;
                    return b;
            }
            int x = 2 * pixelPosition.x;
            float g = Texture.Load(int3(x, pixelPosition.y, 0)).g;
            return g;*/
            
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