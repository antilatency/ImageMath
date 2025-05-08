using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record TransparencyInfill: TextureOperation {
        public float Power = 1.0f;

        public TransparencyInfill(Texture texture) : base(texture) { }
        
        public TransparencyInfill() : base() { }

        public static string GetFragmentShaderBody() => LoadCode("TransparencyInfill.FragmentShaderBody.cginc");
        
        /*{
            return @"
float4 inputColor = Texture.Sample(samplerTexture, input.uv);
@GetColorTransform";
        }*/
    }


}
