using UnityEngine;

namespace ImageMath{
    [FilePath]
    public partial record LambertianSphereFill: FragmentOperation {
        public Vector3 Color;
        public Vector3 LightDirection;
        public LambertianSphereFill(Vector3 lightDirection, Vector3? color = null) : base() {
            Color = color ?? new Vector3(1, 1, 1);
            LightDirection = lightDirection.normalized;
        }
        public LambertianSphereFill() : base() {
            LightDirection = Vector3.forward;
            Color = new Vector3(1, 1, 1);
        }
        public static string GetFragmentShaderBody() => LoadCode("LambertianSphereFill.FragmentShaderBody.cginc");
    }
}
