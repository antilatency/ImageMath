using UnityEngine;
namespace ImageMath{
    public partial record TextureCopy : ColorTransformOperation{
        public TextureCopy(Texture texture) : base(texture) { }
        public TextureCopy() : base() { }
        public static string GetColorTransform() {
            return @"return inputColor;";
        }
    }


}