using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record ShrinkGrow: TextureOperation {
        public float Power {get; set;} = 1.0f;

        public ShrinkGrow(Texture texture) : base(texture) { }
        
        public ShrinkGrow() : base() { }

        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(ShrinkGrow)}.FragmentShaderBody.cginc");
        
        /*{
            return @"
float4 inputColor = Texture.Sample(samplerTexture, input.uv);
@GetColorTransform";
        }*/
    }


}
