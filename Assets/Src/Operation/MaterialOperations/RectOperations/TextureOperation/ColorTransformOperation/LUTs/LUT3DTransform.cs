#nullable enable
using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record LUT3DTransform : ColorTransformOperation {
        private LUT3D _lut3d;
        public Texture3D LUT => _lut3d.Texture;
        public Vector3 DomainMin => _lut3d.DomainMin;
        public Vector3 DomainMax => _lut3d.DomainMax;

        public LUT3DTransform(Texture texture, LUT3D lut3D) : base(texture) {
            _lut3d = lut3D;
        }
#if UNITY_EDITOR
        public static string GetColorTransform() => IncludeOrEmbed("LUT3DTransform.ColorTransform.cginc");
#endif
    }

}
