using UnityEngine;
#nullable enable
namespace ImageMath {
    [FilePath]
    public abstract partial record TextureTextureColorOperation : TextureTextureOperation {
        public TextureTextureColorOperation(Texture textureA, Texture textureB) : base(textureA, textureB) { }
        public TextureTextureColorOperation() : base() { }
        public static string GetFragmentShaderBody() {
            return @"
float4 inputColorA = TextureA.Sample(samplerTextureA, input.uv);
float4 inputColorB = TextureB.Sample(samplerTextureB, input.uv);
@GetColorTransform";
        }
    }

}
