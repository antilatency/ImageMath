using UnityEngine;
using UnityEngine.UI;
namespace ImageMath{

    [FilePath]
    public partial record Denoiser : MaterialOperation {

        public enum KernelSize {
            _3x3,
            _5x5,
            _7x7,
        }

        public Texture Texture { get; set; } = null;
        public float Level { get; set; } = 0.15f;

        [MulticompileOptions]
        public KernelSize Size { get; set; } = KernelSize._3x3;

        [MulticompileOptions]
        public bool RenderDelta { get; set; } = false;

        public Denoiser(Texture texture) {
            Texture = texture;
        }

        public Denoiser() : base() { }




        private static float[] GetDCTBases(int n) {
            var dct = new DiscreteCosineTransform(n);
            float[] bases = new float[n * n];
            float m = Mathf.PI / n;
            for (int u = 0; u < n; u++) {
                for (int x = 0; x < n; x++) {
                    bases[x * n + u] = dct.Basis1D(x, u);
                }
            }
            return bases;
        }

        private static string GetDCTBasesCode(int r) {
            int n = r * 2 + 1;
            var bases = GetDCTBases(n);
            string code = @$"
#define dctR {r}
#define dctN {n}
#define dctIN {1.0f / n}
#define dctM {Mathf.PI / n}
static const float dctBases[{n * n}] = {{{string.Join(",", bases)}}};";
            return code;
        }

#if UNITY_EDITOR
        public static string GetCustomCode() {
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var size in (KernelSize[])System.Enum.GetValues(typeof(KernelSize))) {
                int r = (int)size + 1;
                stringBuilder.AppendLine($"#ifdef Size_{size}");
                stringBuilder.AppendLine(GetDCTBasesCode(r));
                stringBuilder.AppendLine("#endif");
            }
            stringBuilder.AppendLine(Embed($"DiscreteCosineTransform.cginc"));
            return stringBuilder.ToString();
        }

        public static string GetFragmentShaderBody() => Embed($"{nameof(Denoiser)}.FragmentShaderBody.cginc");
 #endif
    }


}
