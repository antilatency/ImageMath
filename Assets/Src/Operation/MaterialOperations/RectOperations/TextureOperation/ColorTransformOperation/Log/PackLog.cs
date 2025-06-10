using UnityEngine;
#nullable enable
namespace ImageMath {
    [FilePath]
    public partial record PackLog : ColorTransformOperation {
        public float WhiteLevel = 1f;
        public float BlackLevel { get; set; } = 0;
        public float ExponentScale = 9.0f;
        public float InverseExponentScale => 1 / ExponentScale;

        public float Multiplier => Mathf.Pow(2, ExponentScale * (WhiteLevel - BlackLevel)) - 1;

        public PackLog(Texture texture) : base(texture) { }
        public PackLog() : base() { }

        public static string GetColorTransform() {
            return @"
float4 x = inputColor;
x.rgb = log2(x.rgb * Multiplier + 1) * InverseExponentScale + BlackLevel;
return x;";
        }

        static float ConvertInternal(float x, float b, float ies, float mult) {
            x = Mathf.Log(x * mult + 1, 2) * ies + b;
            return x;
        }

        public override float Convert(float x) {
            x = ConvertInternal(x, BlackLevel, InverseExponentScale, Multiplier);
            return x;
        }

        public override Vector4 Convert(Vector4 x) {
            float b = BlackLevel;
            float ies = InverseExponentScale;
            float mult = Multiplier;
            x.x = ConvertInternal(x.x, b, ies, mult);
            x.y = ConvertInternal(x.y, b, ies, mult);
            x.z = ConvertInternal(x.z, b, ies, mult);
            return x;
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new UnpackLog {
                Texture = texture,
                WhiteLevel = WhiteLevel,
                BlackLevel = BlackLevel,
                ExponentScale = ExponentScale
            };
        }
    }
}
