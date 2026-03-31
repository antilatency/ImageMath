using UnityEngine;

namespace ImageMath {
    [FilePath]
    public partial record MaximumByAlphaOperation : ReductionOperation {
        public MaximumByAlphaOperation(Texture texture) : base(texture) { }
        public MaximumByAlphaOperation() : base() { }

        public static string GetOperation() => "a = (b.a > a.a) ? b : a;";
    }
}
