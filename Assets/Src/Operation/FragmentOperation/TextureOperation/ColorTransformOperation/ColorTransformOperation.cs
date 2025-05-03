using UnityEngine;
namespace ImageMath{
    public abstract partial record ColorTransformOperation: TextureOperation {
        public ColorTransformOperation(Texture texture) : base(texture) { }
        public ColorTransformOperation() : base() { }
        public static string GetFragmentShaderBody() {
            return @"
float4 inputColor = Texture.Sample(samplerTexture, input.uv);
@GetColorTransform";
        }
    }
}