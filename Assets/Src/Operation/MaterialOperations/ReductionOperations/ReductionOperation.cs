using UnityEngine;
using static ImageMath.Static;

namespace ImageMath {
    [FilePath]
    public abstract partial record ReductionOperation : MaterialOperation {
        public Texture Texture { get; set; }
        public ReductionOperation(Texture texture) {
            Texture = texture;
        }
        public ReductionOperation() : base() { }
#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => Embed("ReductionOperation.FragmentShaderBody.cginc");
        public static string GetInitialization() => "";
        public static string GetFinalization() => "";
#endif

        public Color RenderToPixel() {
            using var average = GetTempRenderTexture(1);
            new AverageWeightedByAlphaOperation(Texture).AssignTo(average);
            return average.Value.GetPixels()[0];
        }
    }
}