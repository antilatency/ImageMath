using UnityEngine;
#nullable enable
namespace ImageMath{
    [FilePath]
    public abstract partial record ColorTransformOperation : TextureOperation {
        public ColorTransformOperation(Texture texture) : base(texture) { }
        public ColorTransformOperation() : base() { }
        public static string GetFragmentShaderBody() {
            return @"
float4 inputColor = Texture.Sample(samplerTexture, input.uv);
@GetColorTransform";
        }

        public virtual Vector4 Convert(Vector4 inputColor) {
            throw new System.NotImplementedException($"{GetType().Name} does not implement Convert method.");
        }
        public Vector3 Convert(Vector3 inputColor) {
            return (Vector3)Convert(new Vector4(inputColor.x, inputColor.y, inputColor.z, 1.0f));
        }

        public virtual ColorTransformOperation CreateInversed(Texture? texture = null) {
            throw new System.NotImplementedException($"{GetType().Name} does not implement CreateInversed method.");
        }
    }
}