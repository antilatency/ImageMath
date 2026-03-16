using System;

using Scopes;
using Scopes.C;

using UnityEngine;
#nullable enable

namespace ImageMath {
    [FilePath]
    public partial record Unpack422 : TextureOperation {

        [MulticompileOptions]
        public Layout422 Layout { get; set; } = Layout422.Cb0Y0Cr0Y1;

        [MulticompileOptions]
        public bool FlipVertically { get; set; } = false;

        public static Vector2Int GetOutputTextureDimensions(Vector2Int inputDimensions, Layout422 layout) {
            switch (layout) {
                case Layout422.Cb0Y0Cr0Y1:
                    return new Vector2Int(inputDimensions.x / 2, inputDimensions.y);
                default:
                    throw new ArgumentOutOfRangeException(nameof(layout), layout, null);
            }
        }

        public enum UpsamplingAlgorithm {
            NearestNeighbor,
            LinearInterpolation,
            GradientBased
        }
        public float AntiNoiseProtection { get; set; } = 0.01f;

        [MulticompileOptions]
        public UpsamplingAlgorithm Algorithm { get; set; } = UpsamplingAlgorithm.LinearInterpolation;

        public Unpack422(Texture? texture = null) : base(texture) {
        }

#if UNITY_EDITOR
        /*
        uint2 outputDimensions(uint w, uint h)
        bool isMajor(uint x, uint y)

        uint2 majorY(uint x, uint y, uint w, uint h)
        uint2 majorCb(uint x, uint y, uint w, uint h)
        uint2 majorCr(uint x, uint y, uint w, uint h)
        uint2 minorY(uint x, uint y, uint w, uint h)
        uint2 minorYLeft(uint x, uint y, uint w, uint h)
        uint2 minorYRight(uint x, uint y, uint w, uint h)
        uint2 minorCbLeft(uint x, uint y, uint w, uint h)
        uint2 minorCbRight(uint x, uint y, uint w, uint h)
        uint2 minorCrLeft(uint x, uint y, uint w, uint h)
        uint2 minorCrRight(uint x, uint y, uint w, uint h)


        */
        private struct LayoutParser {
            public string outputDimensions;
            public string IsMajor;
            public string MajorY;
            public string MajorCb;
            public string MajorCr;
            public string MinorY;
            public string MinorYLeft;
            public string MinorYRight;
            public string MinorCbLeft;
            public string MinorCbRight;
            public string MinorCrLeft;
            public string MinorCrRight;
        }

        private static LayoutParser GetLayoutParser(Layout422 layout) {
            switch (layout) {
                case Layout422.Cb0Y0Cr0Y1:
                    return new LayoutParser {
                        outputDimensions = "uint2(w / 2, h)",
                        IsMajor = "x % 2 == 0",
                        MajorY =  "uint2(2*x+1, y)",
                        MajorCb = "uint2(2*x+0, y)",
                        MajorCr =  "uint2(2*x+2, y)",

                        MinorY = "uint2(2*x+1, y)",
                        MinorYLeft = "uint2(2*x - 1, y)",
                        MinorYRight = "uint2(2*x + ((x+1)<w ? 3 : 1), y)",
                        MinorCbLeft = "uint2(2*x - 2, y)",
                        MinorCbRight = "uint2(2*x + ((x+1)<w ? 2 : -2), y)",
                        MinorCrLeft = "uint2(2*x, y)",
                        MinorCrRight = "uint2(2*x + ((x+1)<w ? 4 : 0), y)",
                    };
                default:
                    throw new ArgumentOutOfRangeException(nameof(layout), layout, null);
            }
        }

        public static string GetCustomCode() {
            var outputDimensions = new Scope("uint2 outputDimensions(uint w, uint h)");
            var isMajor = new Scope("bool isMajor(uint x, uint y)");
            var majorY = new Scope("uint2 majorY(uint x, uint y, uint w, uint h)");
            var majorCb = new Scope("uint2 majorCb(uint x, uint y, uint w, uint h)");
            var majorCr = new Scope("uint2 majorCr(uint x, uint y, uint w, uint h)");
            var minorY = new Scope("uint2 minorY(uint x, uint y, uint w, uint h)");
            var minorYLeft = new Scope("uint2 minorYLeft(uint x, uint y, uint w, uint h)");
            var minorYRight = new Scope("uint2 minorYRight(uint x, uint y, uint w, uint h)");
            var minorCbLeft = new Scope("uint2 minorCbLeft(uint x, uint y, uint w, uint h)");
            var minorCbRight = new Scope("uint2 minorCbRight(uint x, uint y, uint w, uint h)");
            var minorCrLeft = new Scope("uint2 minorCrLeft(uint x, uint y, uint w, uint h)");
            var minorCrRight = new Scope("uint2 minorCrRight(uint x, uint y, uint w, uint h)");
            for (int i = 0; i < Enum.GetValues(typeof(Layout422)).Length; i++) {
                var layout = (Layout422)i;
                var parser = GetLayoutParser(layout);
                outputDimensions.AddRange($"#ifdef Layout_{layout}", $"return {parser.outputDimensions};", "#endif");
                isMajor.AddRange($"#ifdef Layout_{layout}",  $"return {parser.IsMajor};", "#endif");
                majorY.AddRange($"#ifdef Layout_{layout}", $"return {parser.MajorY};", "#endif");
                majorCb.AddRange($"#ifdef Layout_{layout}", $"return {parser.MajorCb};", "#endif");
                majorCr.AddRange($"#ifdef Layout_{layout}", $"return {parser.MajorCr};", "#endif");
                minorY.AddRange($"#ifdef Layout_{layout}", $"return {parser.MinorY};", "#endif");
                minorYLeft.AddRange($"#ifdef Layout_{layout}", $"return {parser.MinorYLeft};", "#endif");
                minorYRight.AddRange($"#ifdef Layout_{layout}", $"return {parser.MinorYRight};", "#endif");
                minorCbLeft.AddRange($"#ifdef Layout_{layout}", $"return {parser.MinorCbLeft};", "#endif");
                minorCbRight.AddRange($"#ifdef Layout_{layout}", $"return {parser.MinorCbRight};", "#endif");
                minorCrLeft.AddRange($"#ifdef Layout_{layout}", $"return {parser.MinorCrLeft};", "#endif");
                minorCrRight.AddRange($"#ifdef Layout_{layout}", $"return {parser.MinorCrRight};", "#endif");
            }

            return new Group {
                outputDimensions,
                isMajor,
                majorY,
                majorCb,
                majorCr,
                minorY,
                minorYLeft,
                minorYRight,
                minorCbLeft,
                minorCbRight,
                minorCrLeft,
                minorCrRight,
                IncludeOrEmbed($"{nameof(Unpack422)}.GradientInterpolator.cginc")
            }.ToString();
        }
        

        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(Unpack422)}.FragmentShaderBody.cginc");

        
#endif
    }

}
