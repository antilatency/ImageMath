Shader "ImageMath/DisplayLinearityValidation"{
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



        VSO vert(VSI input) {
            VSO result;
            result.position = input.position;
        	result.position = UnityObjectToClipPos(result.position);
            result.uv = input.uv;
            return result;
        }
        
        float4 frag(VSO input) : SV_Target {
            int3 pixelPosition = int3(input.position.xy, 0);
            int cellSize = 128;
            int border = 32;
            uint2 cellIndex2D = uint2(pixelPosition.xy) / cellSize;
            
            uint2 innerCellPosition = uint2(pixelPosition.xy) % cellSize;
            static const int numOptions = 4;
            static const float3 aOptions[numOptions] = {1.0.xxx, 0.5.xxx, 0.25.xxx, 1.0.xxx};
            static const float3 bOptions[numOptions] = {0.0.xxx, 0.0.xxx, 0.0.xxx, 0.5.xxx};
            static const float3 multiplierOptions[numOptions] = {float3(1,1,1), float3(1,0,0), float3(0,1,0), float3(0,0,1)};
            float3 multiplier = multiplierOptions[cellIndex2D.y % numOptions];
            float3 a = aOptions[cellIndex2D.x % numOptions] * multiplier;
            float3 b = bOptions[cellIndex2D.x % numOptions] * multiplier;
            float3 m = 0.5 * (a+b);
            if (any(innerCellPosition< border) || any(innerCellPosition>=(cellSize - border))) {
                return float4(m, 1);
            }
            int checker = (pixelPosition.x + pixelPosition.y) % 2;
            return float4(lerp(a,b, checker), 1);
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