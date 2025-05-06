using UnityEngine;

namespace ImageMath{
    [FilePath]
    public partial record MaxOperation : ReductionOperation{
        public MaxOperation(Texture texture) : base(texture) { }
        public MaxOperation() : base() { }
        public static string GetOperation() => "a = max(a, b);";
    }
}