using System;

using Scopes;
using Scopes.C;

using UnityEngine;
#nullable enable

namespace ImageMath {

    public enum Layout422 {
        Cb0Y0Cr0Y1
    }


    [FilePath]
    public partial record Pack422 : TextureOperation {

        [MulticompileOptions]
        public Layout422 Layout { get; set; } = Layout422.Cb0Y0Cr0Y1;

        public static Vector2Int GetOutputTextureDimensions(Vector2Int inputDimensions, Layout422 layout) {
            switch (layout) {
                case Layout422.Cb0Y0Cr0Y1:
                    return new Vector2Int(inputDimensions.x * 2, inputDimensions.y);
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(layout), layout, null);
            }
        }
            

        public Pack422(Texture? texture = null) : base(texture) {
        }

#if UNITY_EDITOR
        /*
            uint sourceComponent(uint x, uint y, uint w, uint h)
            uint2 sourcePosition(uint x, uint y, uint w, uint h)
        */

        private struct LayoutPacker {
            public string outputDimensions;
            public string sourcePosition;
            public string sourceComponent;            
        }

        private static LayoutPacker GetLayoutPacker(Layout422 layout) {
            switch (layout) {
                case Layout422.Cb0Y0Cr0Y1:
                    return new LayoutPacker {
                        outputDimensions = "uint2(w * 2, h)",
                        sourcePosition = "uint2(((x/4*2) + uint4(0,0,0,1))[x%4], y)",
                        sourceComponent = "uint4(1,0,2,0)[x%4];"
                    };
                default:
                    throw new System.ArgumentOutOfRangeException(nameof(layout), layout, null);
            }
        }

        public static string GetCustomCode() {
            var outputDimensions = new Scope("uint2 outputDimensions(uint w, uint h)");
            var sourcePosition = new Scope("uint2 sourcePosition(uint x, uint y, uint w, uint h)");
            var sourceComponent = new Scope("uint sourceComponent(uint x, uint y, uint w, uint h)");
            for (int i = 0; i < Enum.GetValues(typeof(Layout422)).Length; i++) {
                var layout = (Layout422)i;
                var packer = GetLayoutPacker(layout);
                outputDimensions.AddRange($"#ifdef Layout_{layout}", $"return {packer.outputDimensions};", "#endif");
                sourcePosition.AddRange($"#ifdef Layout_{layout}", $"return {packer.sourcePosition};", "#endif");
                sourceComponent.AddRange($"#ifdef Layout_{layout}", $"return {packer.sourceComponent}", "#endif");
            }
            return new Group { 
                outputDimensions,
                sourcePosition,
                sourceComponent
            }.ToString();

        }

        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(Pack422)}.FragmentShaderBody.cginc");
#endif
    }
}
