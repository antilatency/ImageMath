Shader "ImageMath/Pack422"{
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
            uint2 halfTextureSize = textureSize / 2;
            uint2 pixelPosition = input.position.xy;
            /*
            float2 uv = input.uv;
            if (uv.y > 0.5){
                uv.y = (uv.y - 0.5) * 2.0;
                return Texture.Sample(samplerTexture, uv).r;
            }
            uv.y = uv.y * 2.0;
            if (uv.x > 0.5){
                uv.x = (uv.x - 0.5) * 2.0;
                return Texture.Sample(samplerTexture, uv).b;
            } else {
                uv.x = uv.x * 2.0;
                return Texture.Sample(samplerTexture, uv).g;
            }*/
            if (pixelPosition.y >= textureSize.y){
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
            return g;
            
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