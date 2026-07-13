Shader "ImageMath/LUT3DTransform"{
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

        Texture3D<float4> LUT;
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
            //hlsl include to apply a 3D LUT to a color
            // This shader code applies a 3D LUT (Look-Up Table) to a color input.
            // The LUT is a 3D texture that maps input colors to output colors based on the LUT's data.
            // The LUT is sampled using the input color's RGB values, which are transformed into 3D texture coordinates.
            // The resulting color is then returned as the output.
            // The LUT is expected to be a 3D texture with dimensions that are a power of two.
            //get size of LUT
            uint sizeX;
            uint sizeY;
            uint sizeZ;
            LUT.GetDimensions(sizeX, sizeY, sizeZ);
            float3 size = float3(sizeX, sizeY, sizeZ);
            //inverse lerp define
            #define InverseLerp(a, b, t) ((t - a) / (b - a))
            float3 uvw = InverseLerp(DomainMin, DomainMax, inputColor.rgb);
            uvw = clamp(uvw, 0, 1);
            float3 invSize = 1.0 / size;
            float3 offset = 0.5 * invSize;
            float3 scale = 1 - invSize;
            uvw = uvw * scale + offset;
            float4 color = LUT.SampleLevel(samplerLUT, uvw, 0);
            return color;
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