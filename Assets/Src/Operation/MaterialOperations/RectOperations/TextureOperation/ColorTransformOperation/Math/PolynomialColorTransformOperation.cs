

using UnityEngine;
#nullable enable

#if UNITY_EDITOR
using Scopes;
using Scopes.C;
#endif

namespace ImageMath {
    [FilePath]
    public abstract partial record NormalizableColorTransformOperation : ColorTransformOperation {
        public float WhiteLevel = 1f;



        public enum WUsageOptions {
            Transform,
            UseForNormalization
        }
        [MulticompileOptions]
        public WUsageOptions WUsage { get; set; } = WUsageOptions.UseForNormalization;

        public NormalizableColorTransformOperation(Texture texture) : base(texture) { }

        protected NormalizableColorTransformOperation() { }

#if UNITY_EDITOR

        public static string GetCustomCode() {
            return new Group() {
                new Scope("float4 PrepareNormalization(float4 inputColor)") {
                    "#if WUsage_Transform",
                    "return inputColor;",
                    "#elif WUsage_UseForNormalization",
                    "return float4(inputColor.rgb, WhiteLevel);",
                    "#endif"
                },
                new Scope("float4 FinalizeNormalization(float4 outputColor, float inputAlpha)") {
                    "#if WUsage_Transform",
                    "return outputColor;",
                    "#elif WUsage_UseForNormalization",
                    "return float4(outputColor.rgb / outputColor.a, inputAlpha);",
                    "#endif"
                },

            }.ToString();
        }

#endif
    }


    [FilePath]
    public partial record PolynomialColorTransformOperation : NormalizableColorTransformOperation {
        [DynamicArray(16)]
        public float[] Coefficients { get; set; }


        public PolynomialColorTransformOperation(Texture texture, float[] coefficients) : base(texture) {
            Coefficients = coefficients;
        }
        public PolynomialColorTransformOperation(int n) : base() {
            Coefficients = new float[n];
        }
        public static string GetColorTransform() {
            return @"
float4 x = PrepareNormalization(inputColor);
float4 v = 1.0;
float4 result = 0.0;
for (int i = 0; i < Coefficients_Size; i++) {
    result += Coefficients[i] * v;
    v *= x;
}
return FinalizeNormalization(result, inputColor.a);
";
        }


        public override float Convert(float x) {
            float v = 1f;
            float result = 0f;
            for (int i = 0; i < Coefficients.Length; i++) {
                result += Coefficients[i] * v;
                v *= x;
            }
            return result;
        }

        public override Vector4 Convert(Vector4 x) {
            x.x = Convert(x.x);
            x.y = Convert(x.y);
            x.z = Convert(x.z);
            return x;
        }

        public delegate double[] SolverDelegate(double[,] A, double[] B);

        public void Solve(Vector2[] points, SolverDelegate solverDelegate) {
            int n = Coefficients.Length;
            var hessian = new double[n, n];
            var gradient = new double[n];
            foreach (var p in points) {
                double x = p.x;
                double y = p.y;

                double[] phi = new double[n];
                double v = 1.0;
                for (int i = 0; i < n; i++) {
                    phi[i] = v;
                    v *= x;
                }

                for (int i = 0; i < n; i++) {
                    gradient[i] += phi[i] * y;
                    for (int j = 0; j < n; j++) {
                        hessian[i, j] += phi[i] * phi[j];
                    }
                }
            }
            var coefficients = solverDelegate(hessian, gradient);
            for (int i = 0; i < n; i++) {
                Coefficients[i] = (float)coefficients[i];
            }
        }


        public override ColorTransformOperation CreateInverse(Texture? texture = null) {
            throw new System.NotImplementedException();
        }
    }



}
