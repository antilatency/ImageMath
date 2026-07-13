Shader "ImageMath/LUT2DTransform"{
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

        Texture2D<float4> LUT;
        SamplerState samplerLUT;
        float3 DomainMin;
        float3 DomainMax;
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
            float4 inputColor = Texture.Sample(samplerTexture, input.uv);
            //LUT Texture2D<float4>
            //inputColor: float4
            uint sizeX;
            uint sizeY;
            LUT.GetDimensions(sizeX, sizeY);
            float3 color = inputColor.rgb;
            float sum = color.r + color.g + color.b;
            if (sum == 0)
                return float4(0,0,0,1);
            float3 projected = color / sum;
            float2 pixelPosition = projected.xy * float2(sizeX - 2, 2 * sizeY - 1);
            int2 pixelPosition00 = int2(pixelPosition);
            int2 pixelPosition10 = pixelPosition00 + int2(1, 0);
            int2 pixelPosition01 = pixelPosition00 + int2(0, 1);
            float2 frac = pixelPosition - pixelPosition00;
            //return float4(frac,0,1);
            if (frac.x + frac.y > 1) {
                frac = (1 - frac).yx;
                pixelPosition00 += int2(1,1);
            }
            if (pixelPosition00.y >= sizeY) {
                pixelPosition00 = int2(sizeX-1 ,2*sizeY-1) - pixelPosition00;
            }
            if (pixelPosition10.y >= sizeY) {
                pixelPosition10 = int2(sizeX-1 ,2*sizeY-1) - pixelPosition10;
            }
            if (pixelPosition01.y >= sizeY) {
                pixelPosition01 = int2(sizeX-1 ,2*sizeY-1) - pixelPosition01;
            }
            float3 transformed00 = LUT.Load(int3(pixelPosition00, 0)).rgb * sum;
            float3 transformed10 = LUT.Load(int3(pixelPosition10, 0)).rgb * sum;
            float3 transformed01 = LUT.Load(int3(pixelPosition01, 0)).rgb * sum;
            return float4(transformed00 + frac.x * (transformed10 - transformed00) + frac.y * (transformed01 - transformed00), 1);
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