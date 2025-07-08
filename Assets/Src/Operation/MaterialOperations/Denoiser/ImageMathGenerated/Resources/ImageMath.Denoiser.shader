Shader "ImageMath/Denoiser"{
    Properties {}
    SubShader {
        Cull Off ZWrite Off ZTest Always
        ColorMask[ImageMath_ChannelMask]

        CGINCLUDE

        #pragma multi_compile_local Size__3x3 Size__5x5 Size__7x7
        #pragma multi_compile_local _ RenderDelta
        
        #include "UnityCG.cginc"

        #define Pi 3.1415926535897932384626433832795
        #define SquareRootOf2 1.4142135623730950488016887242097
        #define Epsilon 10e-6

        #ifdef Size__3x3
        #define dctR 1
        #define dctN 3
        #define dctIN 0.3333333
        #define dctM 1.047198
        static const float dctBases[9] = {1,0.8660254,0.5,1,-4.371139E-08,-1,1,-0.8660254,0.4999999};
        #endif
        #ifdef Size__5x5
        #define dctR 2
        #define dctN 5
        #define dctIN 0.2
        #define dctM 0.6283185
        static const float dctBases[25] = {1,0.9510565,0.809017,0.5877852,0.309017,1,0.5877852,-0.309017,-0.9510566,-0.8090169,1,-4.371139E-08,-1,1.192488E-08,1,1,-0.5877852,-0.3090171,0.9510564,-0.8090169,1,-0.9510566,0.8090172,-0.5877857,0.3090177};
        #endif
        #ifdef Size__7x7
        #define dctR 3
        #define dctN 7
        #define dctIN 0.1428571
        #define dctM 0.448799
        static const float dctBases[49] = {1,0.9749279,0.9009688,0.7818314,0.6234897,0.4338836,0.2225209,1,0.7818314,0.2225209,-0.4338838,-0.9009689,-0.9749278,-0.6234896,1,0.4338836,-0.62349,-0.9749278,-0.2225205,0.7818317,0.9009687,1,-1.629207E-07,-1,4.887621E-07,1,-8.146034E-07,-1,1,-0.4338838,-0.6234896,0.974928,-0.2225213,-0.7818313,0.9009693,1,-0.7818317,0.2225215,0.4338832,-0.9009684,0.9749282,-0.6234908,1,-0.974928,0.9009691,-0.7818319,0.6234905,-0.4338848,0.2225223};
        #endif
        #include "D:\ImageMath\Assets\Src\Operation\MaterialOperations\Denoiser\DiscreteCosineTransform.cginc"

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

        Texture2D<float4> Texture;
        SamplerState samplerTexture;
        float Power;

        VSO vert(VSI input) {
            VSO result;
            result.position = input.position;
        	result.position = UnityObjectToClipPos(result.position);
            result.uv = input.uv;
            return result;
        }
        
        float4 frag(VSO input) : SV_Target {
            #include "D:\ImageMath\Assets\Src\Operation\MaterialOperations\Denoiser\Denoiser.FragmentShaderBody.cginc"
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