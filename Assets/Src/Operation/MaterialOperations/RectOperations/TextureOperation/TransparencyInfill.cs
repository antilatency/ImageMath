using System.Collections;

using UnityEngine;
namespace ImageMath {
    [FilePath]
    public partial record TransparencyInfill : TextureOperation {
        public float Power { get; set; } = 1.0f;

        public TransparencyInfill(Texture texture) : base(texture) { }

        public TransparencyInfill() : base() { }

        public static string GetFragmentShaderBody() => LoadCode($"{nameof(TransparencyInfill)}.FragmentShaderBody.cginc");
    }


}
