using UnityEngine;
#nullable enable

namespace ImageMath {
    [FilePath]
    public partial record Finetune422 : MaterialOperation {

        public Vector3 YAxisInRGBSpace { get; set; }  = YUV.BT709_RGBFromYCbCr.MultiplyVector(new Vector3(1, 0, 0));

        public Texture? LinearTexture { get; set; }
        public Texture? GammaTexture { get; set; }

        public Finetune422(Texture? linearTexture, Texture? gammaTexture) {
            LinearTexture = linearTexture;
            GammaTexture = gammaTexture;
        }

        public Finetune422(Texture? linearTexture, Texture? gammaTexture, Vector3 yAxisInRGBSpace) {
            LinearTexture = linearTexture;
            GammaTexture = gammaTexture;
            YAxisInRGBSpace = yAxisInRGBSpace;
        }
        
        /*public static string GetCustomCode() {
            return "SamplerState  sampler_Linear_Clamp;";
        }*/

        #if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(Finetune422)}.FragmentShaderBody.cginc");
        #endif
    }

}
