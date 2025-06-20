using UnityEngine;
using UnityEngine.UI;
namespace ImageMath{
    [FilePath]
    public partial record Denoiser: MaterialOperation {
        
        public Texture Texture { get; set; } = null;
        public float Power { get; set; } = 0.15f;
        public int Size { get; set; } = 2;

        public Denoiser(Texture texture) {
            Texture = texture;
        }
        
        public Denoiser() : base() { }

        public static string GetFragmentShaderBody() => Include($"{nameof(Denoiser)}.FragmentShaderBody.cginc");
    }


}
