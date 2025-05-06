using UnityEngine;

namespace ImageMath{
    [FilePath]
    public partial record MinOperation : ReductionOperation{
        public MinOperation(Texture texture) : base(texture) { }
        public MinOperation() : base() { }
        public static string GetOperation() => "a = min(a, b);";
    }
}