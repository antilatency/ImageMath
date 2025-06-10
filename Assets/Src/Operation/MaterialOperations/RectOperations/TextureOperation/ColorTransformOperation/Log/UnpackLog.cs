using UnityEngine;
#nullable enable
namespace ImageMath {
    [FilePath]
    public partial record UnpackLog : ColorTransformOperation {
        public float WhiteLevel = 1f;
        public float BlackLevel { get; set; } = 0;
        public float ExponentScale { get; set; } = 9.0f;


        public float Multiplier => 1 / (Mathf.Pow(2, ExponentScale * (WhiteLevel - BlackLevel)) - 1);


        public UnpackLog(Texture texture) : base(texture) { }
        public UnpackLog() : base() { }
        public static string GetColorTransform() {
            return @"
float4 x = inputColor;
x.rgb = (x.rgb - BlackLevel) * ExponentScale;
x.rgb = pow(2, x.rgb) - 1;
x.rgb *= Multiplier;
return x;";
        }

        static float ConvertInternal(float x, float b, float es, float mult) {
            x = (x - b) * es;
            x = Mathf.Pow(2, x) - 1;
            x *= mult;
            return x;
        }

        public override float Convert(float x) {
            x = ConvertInternal(x, BlackLevel, ExponentScale, Multiplier);
            return x;
        }


        public override Vector4 Convert(Vector4 x) {
            float b = BlackLevel;
            float es = ExponentScale;
            float mul = Multiplier;
            x.x = ConvertInternal(x.x, b, es, mul);
            x.y = ConvertInternal(x.y, b, es, mul);
            x.z = ConvertInternal(x.z, b, es, mul);
            return x;
        }

        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            return new PackLog {
                Texture = texture,
                WhiteLevel = WhiteLevel,
                BlackLevel = BlackLevel,
                ExponentScale = ExponentScale
            };
        }
    }
}