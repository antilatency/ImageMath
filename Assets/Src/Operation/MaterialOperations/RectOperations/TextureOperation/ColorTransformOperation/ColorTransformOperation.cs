using UnityEngine;
#nullable enable
namespace ImageMath {
    [FilePath]
    public abstract partial record ColorTransformOperation : TextureOperation {
        public ColorTransformOperation(Texture texture) : base(texture) { }
        public ColorTransformOperation() : base() { }
        public static string GetFragmentShaderBody() {
            return @"
float4 inputColor = Texture.Sample(samplerTexture, input.uv);
@GetColorTransform";
        }

        public virtual float Convert(float x) {
            throw new System.NotImplementedException($"{GetType().Name} does not implement Convert method for float.");
        }

        public virtual Vector4 Convert(Vector4 x) {
            throw new System.NotImplementedException($"{GetType().Name} does not implement Convert method for Vector4.");
        }
        public Vector3 Convert(Vector3 x) {
            return (Vector3)Convert(new Vector4(x.x, x.y, x.z, 1.0f));
        }

        public virtual ColorTransformOperation CreateInverse(Texture? texture = null) {
            throw new System.NotImplementedException($"{GetType().Name} does not implement CreateInversed method.");
        }
    }
}