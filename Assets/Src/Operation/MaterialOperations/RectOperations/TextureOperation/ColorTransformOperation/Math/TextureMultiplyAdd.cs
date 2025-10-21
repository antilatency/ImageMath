using UnityEngine;
#nullable enable
namespace ImageMath{
    [FilePath]
    public partial record TextureMultiplyAdd : ColorTransformOperation {
        public Matrix4x4 Multiplier { get; set; } = Matrix4x4.identity;
        public Vector4 Increment { get; set; } = Vector4.zero;

        public TextureMultiplyAdd(Texture texture, Matrix4x4 multiplier, Vector4 increment) : base(texture) {
            Multiplier = multiplier;
            Increment = increment;
        }

        public TextureMultiplyAdd(Texture texture, Matrix4x4 multiplier) : this(texture, multiplier, Vector4.zero) { }

        static Matrix4x4 MakeScaleMatrix(Vector4 multiplier) {
            var matrix = Matrix4x4.identity;
            matrix.m00 = multiplier.x;
            matrix.m11 = multiplier.y;
            matrix.m22 = multiplier.z;
            matrix.m33 = multiplier.w;
            return matrix;
        }

        public TextureMultiplyAdd(Texture texture, Vector4 multiplier, Vector4 increment)
            : this(texture, MakeScaleMatrix(multiplier), increment) { }

        public TextureMultiplyAdd(Texture texture, Vector4 multiplier)
            : this(texture, MakeScaleMatrix(multiplier), Vector4.zero) { }

        public TextureMultiplyAdd(Texture texture,
            float rgbMultiplier, float rgbIncrement = 0,
            float alphaMultiplier = 1, float alphaIncrement = 0)
            : this(texture,
            new Vector4(rgbMultiplier, rgbMultiplier, rgbMultiplier, alphaMultiplier),
            new Vector4(rgbIncrement, rgbIncrement, rgbIncrement, alphaIncrement)
        ) { }


        public TextureMultiplyAdd(Texture texture) : base(texture) { }
        public TextureMultiplyAdd() : base() { }
        public static string GetColorTransform() {
            return @"
return mul(Multiplier, inputColor) + Increment;";
        }

        public override Vector4 Convert(Vector4 inputColor) {
            return Multiplier * inputColor + Increment;
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            var inverseMultiplier = Multiplier.inverse;
            var inverseIncrement = -(inverseMultiplier * Increment);
            return new TextureMultiplyAdd {
                Texture = texture,
                Multiplier = inverseMultiplier,
                Increment = inverseIncrement
            };
        }


    }


}