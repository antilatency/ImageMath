Shader "ImageMath/PackSRGB"{
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

        Texture2D<float4> ImageMath_T0;
        #define Texture ImageMath_T0
        SamplerState samplerImageMath_T0;
        #define samplerTexture samplerImageMath_T0
        float2 ImageMath_V0;
        #define Position ImageMath_V0
        float2 ImageMath_V1;
        #define Size ImageMath_V1

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
            float3 x = max(0, inputColor.rgb);
            x = x < 0.0031308 ? 12.92 * x : 1.055 * pow(x, 1.0 / 2.4) - 0.055;
            return float4(x.r, x.g, x.b, inputColor.a);
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