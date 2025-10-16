using UnityEngine;
using static ImageMath.Static;
namespace ImageMath {
    [FilePath]
    public partial record TransparencyInfill : TextureOperation {
        public float Power { get; set; } = 1.0f;
        public Vector3 BackupColor { get; set; } = Vector3.zero;

        public TransparencyInfill(Texture texture) : base(texture) { }

        public TransparencyInfill() : base() { }

        //bool IsPowerOfTwo(int x) => (x > 0) && ((x & (x - 1)) == 0);


        public void PreciseAssignTo(RenderTexture renderTexture, bool preciseLastMip = true) {
            if (Texture == null) {
                throw new System.Exception("Texture is null");
            }
            if (renderTexture == null) {
                throw new System.Exception("RenderTexture is null");
            }
            var hasMips = Texture.mipmapCount > 1;
            var isPowerOfTwoWidth = Mathf.IsPowerOfTwo(Texture.width);
            var isPowerOfTwoHeight = Mathf.IsPowerOfTwo(Texture.height);
            if (isPowerOfTwoWidth && isPowerOfTwoHeight && hasMips) {
                AssignTo(renderTexture);
                return;
            }
            var previousPowerOfTwoWidth = Mathf.NextPowerOfTwo(Texture.width) / 2;
            var previousPowerOfTwoHeight = Mathf.NextPowerOfTwo(Texture.height) / 2;
            using var temp = GetTempRenderTexture(previousPowerOfTwoWidth, previousPowerOfTwoHeight, true, FilterMode.Bilinear, RenderTextureFormat.ARGBFloat);
            new TextureCopy(Texture).AssignTo(temp.Value);
            temp.Value.GenerateMips();
            new TransparencyInfill(temp.Value) { Power = Power }.AssignTo(renderTexture);
            if (preciseLastMip)
                new TextureCopy(Texture).PremultipliedAlphaBlendTo(renderTexture);
        }

#if UNITY_EDITOR
        public static string GetCustomCode() {
            return "SamplerState  sampler_Linear_Clamp;";

        }

        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(TransparencyInfill)}.FragmentShaderBody.cginc");
#endif
    }
}
