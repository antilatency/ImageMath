namespace ImageMath {
    [FilePath]
    public partial record TextureDividedByAlpha : ColorTransformOperation {
        public TextureDividedByAlpha(UnityEngine.Texture texture) : base(texture) { }
        public TextureDividedByAlpha() : base() { }

        public static string GetColorTransform() => @"
        float divider = inputColor.a == 0 ? 1 : inputColor.a;
        return inputColor / divider;";
    }
}
