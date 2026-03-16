using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record ClippingDetection : MaterialOperation {
        public Texture Texture { get; set; } = null;

        public ClippingDetection(Texture texture) {
            Texture = texture;
        }

        public ClippingDetection() : base() { }
#if UNITY_EDITOR
        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(ClippingDetection)}.FragmentShaderBody.cginc");
#endif
    }


}
