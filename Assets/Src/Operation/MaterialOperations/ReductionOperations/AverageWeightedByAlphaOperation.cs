using UnityEngine;

namespace ImageMath{
    [FilePath]
    public partial record AverageWeightedByAlphaOperation : ReductionOperation{
        public AverageWeightedByAlphaOperation(Texture texture) : base(texture) { }
        public AverageWeightedByAlphaOperation() : base() { }
        public static new string GetInitialization() => "a.rgb = a.rgb * a.a;";
        public static string GetOperation() => "a += float4(b.rgb * b.a, b.a);";

        public static new string GetFinalization() => "a.rgb /= (a.a + (a.a==0)); a.a /= maxPixelsPerBlock;";

        public static Vector3 DivideByAlpha(Vector4 vector){
            if (vector.w == 0) return Vector3.zero;
            return new Vector3(vector.x / vector.w, vector.y / vector.w, vector.z / vector.w);
        }

        public static Vector3 SoftwareReduction(Vector4[] pixels){
            var sum = Vector4.zero;

            for (int i = 0; i < pixels.Length; i++) {
                var pixel = pixels[i];
                sum += new Vector4(pixel.x * pixel.w, pixel.y * pixel.w, pixel.z * pixel.w, pixel.w);
            }
            return DivideByAlpha(sum);
        }


    }
}