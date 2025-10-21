using UnityEngine;
namespace ImageMath {
    [FilePath]
    public partial record TextureCompare : ColorTransformOperation{
        public enum CompareOperation {
            Equal,
            NotEqual,
            Greater,
            Less,
            GreaterOrEqual,
            LessOrEqual
        }

        public CompareOperation Operation = CompareOperation.Equal;

        public Vector4 Reference { get; set; } = new Vector4(1, 1, 1, 1);

        private int EqualOperation => (Operation == CompareOperation.Equal || Operation == CompareOperation.NotEqual)? 1 : 0;

        private float PreMultiplier => Operation switch {
            CompareOperation.Equal => 1f,
            CompareOperation.NotEqual => 1f,
            CompareOperation.Greater => 1f,
            CompareOperation.Less => -1f,
            CompareOperation.GreaterOrEqual => 1f,
            CompareOperation.LessOrEqual => -1f,
            _ => 1f
        };

        private int PostInverse => Operation switch {
            CompareOperation.Equal => 0,
            CompareOperation.NotEqual => 1,
            CompareOperation.Greater => 0,
            CompareOperation.Less => 0,
            CompareOperation.GreaterOrEqual => 1,
            CompareOperation.LessOrEqual => 1,
            _ => 0
        };


        public TextureCompare(Texture texture, CompareOperation operation, Vector4 reference) : base(texture) {
            Operation = operation;
            Reference = reference;
        }
        public TextureCompare(Texture texture, CompareOperation operation, float reference) : base(texture) {
            Operation = operation;
            Reference = new Vector4(reference, reference, reference, reference);
        }

        public TextureCompare() : base() { }

        public static string GetColorTransform() {
            return $@"
float4 difference = inputColor - {nameof(Reference)};
difference *= {nameof(PreMultiplier)};
float4 result;
if (EqualOperation)
    result = difference == 0;
else
    result = difference > 0;
return {nameof(PostInverse)}?(1-result):result;";
        }
    }
}
