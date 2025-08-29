using System.Net.NetworkInformation;
using UnityEngine;
namespace ImageMath{
    [FilePath]
    public abstract partial record ReductionOperation : MaterialOperation{
        public Texture Texture {get; set;}
        public ReductionOperation(Texture texture) {
            Texture = texture;
        }
        public ReductionOperation() : base() { }
        public static string GetFragmentShaderBody() => Embed("ReductionOperation.FragmentShaderBody.cginc");
        public static string GetInitialization() => "";
        public static string GetFinalization() => "";

    }
}