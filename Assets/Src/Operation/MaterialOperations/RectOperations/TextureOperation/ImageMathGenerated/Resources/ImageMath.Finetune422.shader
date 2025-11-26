Shader "ImageMath/Finetune422"{
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

        float3 YAxisInRGBSpace;
        Texture2D<float4> LinearTexture;
        SamplerState samplerLinearTexture;
        Texture2D<float4> GammaTexture;
        SamplerState samplerGammaTexture;

        VSO vert(VSI input) {
            VSO result;
            result.position = input.position;
        	result.position = UnityObjectToClipPos(result.position);
            result.uv = input.uv;
            return result;
        }
        
        float4 frag(VSO input) : SV_Target {
            uint2 textureSize = 0;
            uint levels = 0;
            LinearTexture.GetDimensions(0, textureSize.x, textureSize.y, levels);
            int2 pixelPosition = input.position.xy;
            float3 l = LinearTexture.Load(int3(pixelPosition,0)).rgb;
            bool evenX = (pixelPosition.x % 2) == 0;
            if (evenX)
                return float4(l, 1.0);
            float3 g = GammaTexture.Load(int3(pixelPosition,0)).rgb;
            float3 scale = l / g;
            float3 planeNormal = normalize(YAxisInRGBSpace * scale);
            float planeD = dot(planeNormal, l);
            float3 prevL = LinearTexture.Load(int3(pixelPosition.x -1, pixelPosition.y,0)).rgb;
            float3 nextL = LinearTexture.Load(int3(pixelPosition.x +1, pixelPosition.y,0)).rgb;
            //intersection of plane and line from prevL to nextL
            float3 lineDir = nextL - prevL;
            float lineDirLength = length(lineDir);
            if (lineDirLength < 0.01){
                return float4(l, 1.0);
            }
            lineDir = lineDir / lineDirLength;
            float d = dot(planeNormal, lineDir);
            if (abs(d) < 0.1){
                return float4(l, 1.0);
            }
            float t = (planeD - dot(planeNormal, prevL)) / d;
            t = clamp(t, 0.0, lineDirLength);
            //return t;
            float3 intersection = prevL + lineDir * t;
            return float4(intersection, 1.0);
            /*
            float r = Texture.Load(int3(pixelPosition.x , pixelPosition.y + halfTextureSize.y, 0)).r;
            float2 gb = float2(
                Texture.Load(int3(pixelPosition.x / 2, pixelPosition.y, 0)).r,
                Texture.Load(int3(pixelPosition.x / 2 + halfTextureSize.x, pixelPosition.y, 0)).r
            );
            bool oddX = (pixelPosition.x % 2) == 1;
            bool evenX = (pixelPosition.x % 2) == 0;
            if (evenX){
                return float4(r, gb, 1.0);
            }
            int nextX = min(pixelPosition.x / 2 + 1, halfTextureSize.x -1);
            float2 gbNext = float2(
                Texture.Load(int3(nextX, pixelPosition.y, 0)).r,
                Texture.Load(int3(nextX + halfTextureSize.x, pixelPosition.y, 0)).r
            );
            float leftR = Texture.Load(int3(pixelPosition.x -1 , pixelPosition.y + halfTextureSize.y, 0)).r;
            float rightR = Texture.Load(int3(pixelPosition.x +1 , pixelPosition.y + halfTextureSize.y, 0)).r;
            float dR = rightR - leftR;
            float absR = abs(dR);
            if (absR < 0.0001){
                float2 gbAvg = (gb + gbNext) * 0.5;
                return float4(r, gbAvg, 1.0);
            }
            float t = (r - leftR) / dR;
            t = clamp(t, 0.0, 1.0);
            float2 absGB = abs(gbNext - gb);
            //return float4(absR, absGB, 1);
            float2 sum = absR + absGB;
            float2 proj = absGB / sum;
            //return float4(proj, 0.0, 1.0);
            float2 tSuppressed = lerp(t, 0.5, pow(proj, Power));
            float2 myGB = lerp(gb, gbNext, tSuppressed);
            return float4(r, myGB, 1.0);
            */
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