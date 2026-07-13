Shader "ImageMath/Unpack422"{
    Properties {}
    SubShader {
        Cull Off ZWrite Off ZTest Always
        ColorMask[ImageMath_ChannelMask]

        CGINCLUDE

        #pragma multi_compile_local Layout_Cb0Y0Cr0Y1
        #pragma multi_compile_local _ FlipVertically
        #pragma multi_compile_local Algorithm_NearestNeighbor Algorithm_LinearInterpolation Algorithm_GradientBased
        
        #include "UnityCG.cginc"

        #define Pi 3.1415926535897932384626433832795
        #define SquareRootOf2 1.4142135623730950488016887242097
        #define Epsilon 10e-6

        uint2 outputDimensions(uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(w / 2, h);
        	#endif
        }
        bool isMajor(uint x, uint y) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return x % 2 == 0;
        	#endif
        }
        uint2 majorY(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x+1, y);
        	#endif
        }
        uint2 majorCb(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x+0, y);
        	#endif
        }
        uint2 majorCr(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x+2, y);
        	#endif
        }
        uint2 minorY(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x+1, y);
        	#endif
        }
        uint2 minorYLeft(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x - 1, y);
        	#endif
        }
        uint2 minorYRight(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x + ((x+1)<w ? 3 : 1), y);
        	#endif
        }
        uint2 minorCbLeft(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x - 2, y);
        	#endif
        }
        uint2 minorCbRight(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x + ((x+1)<w ? 2 : -2), y);
        	#endif
        }
        uint2 minorCrLeft(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x, y);
        	#endif
        }
        uint2 minorCrRight(uint x, uint y, uint w, uint h) {
        	#ifdef Layout_Cb0Y0Cr0Y1
        	return uint2(2*x + ((x+1)<w ? 4 : 0), y);
        	#endif
        }
        float GradientInterpolator(float left, float right, float center, float w) {
            float vx  = right - left;
            float vx2 = vx * vx;
            float hw  = 0.5 * w;
            float sl  = vx2 + 1.0;
            float isl = 1.0 / sl;
            float num = hw + isl * (vx * (center - left) + hw);
            float det = w + (w + vx2) * isl;
            if (abs(det) < 1e-7) {
                return 0.5;
            }
            return num / det;
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

        float AntiNoiseProtection;
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
            uint2 textureSize = 0;
            #define ReadProperty(propertyName) Texture.Load(int3(propertyName(pixelPosition.x, pixelPosition.y, outputSize.x, outputSize.y), 0)).r
            uint levels = 0;
            Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
            uint2 outputSize = outputDimensions(textureSize.x, textureSize.y);
            int2 pixelPosition = input.position.xy;
            #if FlipVertically
            pixelPosition.y = int(outputSize.y) - 1 - pixelPosition.y;
            #endif
            bool major = isMajor(pixelPosition.x, pixelPosition.y);
            float Y;
            float2 CbCr;
            if (major){
                Y = ReadProperty(majorY);
                float Cb = ReadProperty(majorCb);
                float Cr = ReadProperty(majorCr);
                CbCr = float2(Cb, Cr);
            } else { //minor pixel
                Y = ReadProperty(minorY);
                float CbLeft = ReadProperty(minorCbLeft);
                float CrLeft = ReadProperty(minorCrLeft);
                #if Algorithm_NearestNeighbor
                CbCr = float2(CbLeft, CrLeft);
                #else
                float CbRight = ReadProperty(minorCbRight);
                float CrRight = ReadProperty(minorCrRight);
                #if Algorithm_LinearInterpolation
                CbCr = lerp(float2(CbLeft, CrLeft), float2(CbRight, CrRight), 0.5);
                #else
                float YLeft = ReadProperty(minorYLeft);
                float YRight = ReadProperty(minorYRight);
                float t = GradientInterpolator(YLeft, YRight, Y, AntiNoiseProtection);
                t = clamp(t, 0, 1);
                CbCr = lerp(float2(CbLeft, CrLeft), float2(CbRight, CrRight), t);
                #endif
                #endif
            }
            float3 mul = 255.0 / float3(235.0 - 16.0, 240.0 - 16.0, 240.0 - 16.0);
            float3 add = float3(-16.0/ 255.0, -16.0/ 255.0, -(16)/ 255.0);
            float3 yuv = float3(Y, CbCr) + add;
            yuv = yuv * mul;
            return float4(yuv, 1.0);
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