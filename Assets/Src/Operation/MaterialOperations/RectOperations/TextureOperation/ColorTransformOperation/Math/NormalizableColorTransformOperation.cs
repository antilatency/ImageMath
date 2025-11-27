

using UnityEngine;
#nullable enable

#if UNITY_EDITOR
using Scopes;
using Scopes.C;
#endif

namespace ImageMath {
    [FilePath]
    public abstract partial record NormalizableColorTransformOperation : ColorTransformOperation {
        public float WhiteLevel { get; set; } = 1f;

        public enum WUsageOptions {
            Transform,
            UseForNormalization
        }
        [MulticompileOptions]
        public WUsageOptions WUsage { get; set; } = WUsageOptions.UseForNormalization;

        public NormalizableColorTransformOperation(Texture texture) : base(texture) { }

        protected NormalizableColorTransformOperation() { }

#if UNITY_EDITOR

        public static string GetCustomCode() {
            return new Group() {
                new Scope("float4 PrepareNormalization(float4 inputColor, float whiteLevel)") {
                    "#if WUsage_Transform",
                    "return inputColor;",
                    "#elif WUsage_UseForNormalization",
                    "return float4(inputColor.rgb, whiteLevel);",
                    "#endif"
                },
                new Scope("float4 FinalizeNormalization(float4 outputColor, float inputAlpha)") {
                    "#if WUsage_Transform",
                    "return outputColor;",
                    "#elif WUsage_UseForNormalization",
                    "return float4(outputColor.rgb / outputColor.a, inputAlpha);",
                    "#endif"
                },

            }.ToString();
        }

#endif
    }



}
