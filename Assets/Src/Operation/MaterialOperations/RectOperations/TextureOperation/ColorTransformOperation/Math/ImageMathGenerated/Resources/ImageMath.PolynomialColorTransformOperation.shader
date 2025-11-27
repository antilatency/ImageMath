Shader "ImageMath/PolynomialColorTransformOperation"{
    Properties {}
    SubShader {
        Cull Off ZWrite Off ZTest Always
        ColorMask[ImageMath_ChannelMask]

        CGINCLUDE

        #pragma multi_compile_local WUsage_Transform WUsage_UseForNormalization
        
        #include "UnityCG.cginc"

        #define Pi 3.1415926535897932384626433832795
        #define SquareRootOf2 1.4142135623730950488016887242097
        #define Epsilon 10e-6

        float4 PrepareNormalization(float4 inputColor, float whiteLevel) {
        	#if WUsage_Transform
        	return inputColor;
        	#elif WUsage_UseForNormalization
        	return float4(inputColor.rgb, whiteLevel);
        	#endif
        }
        float4 FinalizeNormalization(float4 outputColor, float inputAlpha) {
        	#if WUsage_Transform
        	return outputColor;
        	#elif WUsage_UseForNormalization
        	return float4(outputColor.rgb / outputColor.a, inputAlpha);
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

        float Coefficients[16];
        int Coefficients_Size;
        float WhiteLevel;
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
            float4 x = PrepareNormalization(inputColor, WhiteLevel);
            float4 v = 1.0;
            float4 result = 0.0;
            for (int i = 0; i < Coefficients_Size; i++) {
                result += Coefficients[i] * v;
                v *= x;
            }
            return FinalizeNormalization(result, inputColor.a);
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