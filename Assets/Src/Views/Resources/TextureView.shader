Shader "ImageMath/Views/TextureView" {
    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _SrcBlend("Src Blend", Float) = 5 // SrcAlpha
        _DstBlend("Dst Blend", Float) = 10 // OneMinusSrcAlpha
    }

    SubShader{

        Cull Back
        Blend [_SrcBlend] [_DstBlend]


        Pass {
            Name "Default"
            CGPROGRAM
                #pragma vertex vertexShader
                #pragma fragment fragmentShader
                #pragma target 2.0

                #include "UnityCG.cginc"
                #include "UnityUI.cginc"


                #pragma multi_compile_local Sampler_Point Sampler_Bilinear Sampler_Trilinear


                struct VertexInput {
                    float4 vertex   : POSITION;
                    float2 uv       : TEXCOORD0;
                };

                struct FragmentInput {
                    float4 vertex   : SV_POSITION;
                    float2 uv       : TEXCOORD0;
                };


                

                FragmentInput vertexShader(VertexInput input) {
                    FragmentInput output;

                    output.vertex = UnityObjectToClipPos(input.vertex);
                    output.uv = input.uv;

                    return output;
                }


                Texture2D _MainTex;
                SamplerState point_clamp_sampler;
                SamplerState bilinear_clamp_sampler;
                SamplerState trilinear_clamp_sampler;


                float4 fragmentShader(FragmentInput input) : SV_Target {

                    #ifdef Sampler_Point
                    return _MainTex.Sample(point_clamp_sampler, input.uv);
                    #elif Sampler_Bilinear
                    return _MainTex.Sample(bilinear_clamp_sampler, input.uv);
                    #elif Sampler_Trilinear
                    return _MainTex.Sample(trilinear_clamp_sampler, input.uv);
                    #endif
                }
            ENDCG
        }
    }
}