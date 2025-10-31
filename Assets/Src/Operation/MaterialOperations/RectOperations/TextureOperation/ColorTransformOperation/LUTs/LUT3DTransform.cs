#nullable enable
using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record LUT3DTransform : ColorTransformOperation {
        private Texture3D LUT { get;  set; }
        public Vector3 DomainMin { get;  set; } = Vector3.zero;
        public Vector3 DomainMax { get;  set; } = Vector3.one;

        public LUT3DTransform(Texture texture, Texture3D lut3D) : base(texture) {
            LUT = lut3D;
        }
#if UNITY_EDITOR
        public static string GetColorTransform() => IncludeOrEmbed("LUT3DTransform.ColorTransform.cginc");
#endif
    }

}
