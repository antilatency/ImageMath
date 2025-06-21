Shader "ImageMath/LineFill"{
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

        float4 Color;
        float LineWidth;
        float LineSoftness;
        float2 PointA;
        float2 PointB;
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
            float2 uv = input.uv;
            float2 A = PointA;
            float2 B = PointB;
            float2 AB = B - A;
            float2 ABPerp = normalize(float2(-AB.y, AB.x));
            float2 AUV = uv - A;
            float d = dot(AUV, ABPerp);
            float dotAB = dot(AB, AB);
            float t = dotAB<Epsilon ? 0 : dot(AUV, AB) / dotAB;
            float distance = abs(d);
            if (t<=0) distance = length(AUV);
            if (t>=1) distance = length(uv - B);
            if (distance>LineWidth) return 0;
            distance /= LineWidth;
            distance = LineSoftness<Epsilon ? distance>=1 : pow(distance, 1/LineSoftness);
            float alpha = cos(distance * Pi) * 0.5 + 0.5;
            return float4(Color.rgb,Color.a*alpha);
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