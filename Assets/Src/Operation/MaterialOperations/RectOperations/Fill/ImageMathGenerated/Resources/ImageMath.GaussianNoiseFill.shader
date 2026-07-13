Shader "ImageMath/GaussianNoiseFill"{
    Properties {}
    SubShader {
        Cull Off ZWrite Off ZTest Always
        ColorMask[ImageMath_ChannelMask]

        CGINCLUDE


        
        #include "UnityCG.cginc"

        #define Pi 3.1415926535897932384626433832795
        #define SquareRootOf2 1.4142135623730950488016887242097
        #define Epsilon 10e-6

        // Hash 2D → 1D (good quality)
        float Hash21(float2 p){
            p = frac(p * float2(123.34, 345.45));
            p += dot(p, p + 34.345);
            return frac(p.x * p.y);
        }
        float GaussianNoise(float2 uv){
            // Two independent uniform randoms in (0,1)
            float u1 = Hash21(uv);
            float u2 = Hash21(uv + 37.0);   // shifted uv to decorrelate
            // Avoid log(0)
            u1 = max(u1, 1e-6);
            // Box–Muller transform → Gaussian(mean=0, stddev=1)
            float r = sqrt(-2.0 * log(u1));
            float a = 6.2831853 * u2;       // 2π*u2
            return r * cos(a);
        }
        float4 GaussianNoiseRGBA(float2 uv){
            return float4(
                GaussianNoise(uv + float2(0.0, 0.0)),
                GaussianNoise(uv + float2(12.34, 56.78)),
                GaussianNoise(uv + float2(90.12, 34.56)),
                GaussianNoise(uv + float2(78.90, 12.34))
            );
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

        float Amplitude;
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
            return Amplitude * GaussianNoise(input.uv);
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