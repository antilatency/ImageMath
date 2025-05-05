namespace ImageMath{
    [FilePath]
    public partial record UVFill: FragmentOperation {
        public static string GetFragmentShaderBody() {
            return "return float4(input.uv,0,1);";
        }
    }
}
