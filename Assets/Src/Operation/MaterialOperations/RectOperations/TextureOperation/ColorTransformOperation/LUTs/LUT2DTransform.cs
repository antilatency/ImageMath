#nullable enable
using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record LUT2DTransform : ColorTransformOperation {
        private Texture LUT { get;  set; }
        public Vector3 DomainMin { get;  set; } = Vector3.zero;
        public Vector3 DomainMax { get;  set; } = Vector3.one;

        public LUT2DTransform(Texture texture, Texture lut2D) : base(texture) {
            LUT = lut2D;
        }
#if UNITY_EDITOR
        public static string GetColorTransform() => IncludeOrEmbed("LUT2DTransform.ColorTransform.cginc");
#endif
    }

}
