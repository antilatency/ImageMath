Shader "ImageMath/DotProduct"{
    Properties {}
    SubShader {
        Cull Off ZWrite Off ZTest Always
        ColorMask[ImageMath_ChannelMask]

        CGINCLUDE

        #pragma multi_compile_local Count_1 Count_2 Count_3 Count_4 Count_5 Count_6 Count_7 Count_8 Count_9 Count_10 Count_11 Count_12 Count_13 Count_14 Count_15 Count_16
        
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

        float4 Weights[16];
        #define Weights_Size 16
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
        
        Texture2D<float4> T0;
        SamplerState samplerT0;
        #if Count_2 || Count_3 || Count_4 || Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T1;
        SamplerState samplerT1;
        #endif
        #if Count_3 || Count_4 || Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T2;
        SamplerState samplerT2;
        #endif
        #if Count_4 || Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T3;
        SamplerState samplerT3;
        #endif
        #if Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T4;
        SamplerState samplerT4;
        #endif
        #if Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T5;
        SamplerState samplerT5;
        #endif
        #if Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T6;
        SamplerState samplerT6;
        #endif
        #if Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T7;
        SamplerState samplerT7;
        #endif
        #if Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T8;
        SamplerState samplerT8;
        #endif
        #if Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T9;
        SamplerState samplerT9;
        #endif
        #if Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T10;
        SamplerState samplerT10;
        #endif
        #if Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T11;
        SamplerState samplerT11;
        #endif
        #if Count_13 || Count_14 || Count_15 || Count_16
        Texture2D<float4> T12;
        SamplerState samplerT12;
        #endif
        #if Count_14 || Count_15 || Count_16
        Texture2D<float4> T13;
        SamplerState samplerT13;
        #endif
        #if Count_15 || Count_16
        Texture2D<float4> T14;
        SamplerState samplerT14;
        #endif
        #if Count_16
        Texture2D<float4> T15;
        SamplerState samplerT15;
        #endif
        float4 frag(VSO input) : SV_Target {
        float4 sum = T0.Sample(samplerT0, input.uv) * Weights[0];
        #if Count_2 || Count_3 || Count_4 || Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T1.Sample(samplerT1, input.uv) * Weights[1];
        #endif
        #if Count_3 || Count_4 || Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T2.Sample(samplerT2, input.uv) * Weights[2];
        #endif
        #if Count_4 || Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T3.Sample(samplerT3, input.uv) * Weights[3];
        #endif
        #if Count_5 || Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T4.Sample(samplerT4, input.uv) * Weights[4];
        #endif
        #if Count_6 || Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T5.Sample(samplerT5, input.uv) * Weights[5];
        #endif
        #if Count_7 || Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T6.Sample(samplerT6, input.uv) * Weights[6];
        #endif
        #if Count_8 || Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T7.Sample(samplerT7, input.uv) * Weights[7];
        #endif
        #if Count_9 || Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T8.Sample(samplerT8, input.uv) * Weights[8];
        #endif
        #if Count_10 || Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T9.Sample(samplerT9, input.uv) * Weights[9];
        #endif
        #if Count_11 || Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T10.Sample(samplerT10, input.uv) * Weights[10];
        #endif
        #if Count_12 || Count_13 || Count_14 || Count_15 || Count_16
        sum += T11.Sample(samplerT11, input.uv) * Weights[11];
        #endif
        #if Count_13 || Count_14 || Count_15 || Count_16
        sum += T12.Sample(samplerT12, input.uv) * Weights[12];
        #endif
        #if Count_14 || Count_15 || Count_16
        sum += T13.Sample(samplerT13, input.uv) * Weights[13];
        #endif
        #if Count_15 || Count_16
        sum += T14.Sample(samplerT14, input.uv) * Weights[14];
        #endif
        #if Count_16
        sum += T15.Sample(samplerT15, input.uv) * Weights[15];
        #endif
        return sum;
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