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

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            

                struct VertexInput {
                    float4 vertex   : POSITION;
                    float2 uv       : TEXCOORD0;
                };

                struct FragmentInput {
                    float4 vertex   : SV_POSITION;
                    float2 uv       : TEXCOORD0;
                };


                float4 _ClipRect;
                sampler2D _MainTex;

                FragmentInput vertexShader(VertexInput input) {
                    FragmentInput output;

                    output.vertex = UnityObjectToClipPos(input.vertex);
                    output.uv = input.uv;

                    return output;
                }



                float4 fragmentShader(FragmentInput input) : SV_Target {
                    // Sample the texture using the UV coordinates
                    float4 color = tex2D(_MainTex, input.uv);

                    return color;
                }
            ENDCG
        }
    }
}