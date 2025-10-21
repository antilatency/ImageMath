using System.Collections.Generic;
using System.Linq;

using UnityEngine;
namespace ImageMath {
    [FilePath]
    public partial record DotProduct : RectOperation {
        public const int MaxTextures = 16;
        public IList<Texture> Textures { get; set; }

        [FixedArray(MaxTextures)]
        public IList<Vector4> Weights { get; set; }

        [MulticompileOptions(1,MaxTextures)]
        public int Count => Textures?.Count ?? 0;

        public DotProduct(params (Texture texture, Vector4 weight)[] pairs)
            : this(pairs.Select(x => x.texture).ToList(), pairs.Select(x => x.weight).ToList()) { }

        public DotProduct(params (Texture texture, float weight)[] pairs)
            : this(pairs.Select(x => x.texture).ToList(), pairs.Select(x => new Vector4(x.weight, x.weight, x.weight, x.weight)).ToList()) { }

        public DotProduct(IList<Texture> textures, IList<float> weights)
            : this(textures, weights.Select(x => new Vector4(x, x, x, x)).ToList()) { }


        public DotProduct(IList<Texture> textures, IList<Vector4> weights) {
            if (textures == null || weights == null) {
                throw new System.ArgumentNullException("Textures and Weights cannot be null.");
            }
            if (textures.Count != weights.Count) {
                throw new System.ArgumentException("Textures and Weights must have the same number of elements.");
            }
            if (textures.Count > MaxTextures) {
                throw new System.ArgumentException($"Cannot have more than {MaxTextures} textures.");
            }
            Textures = textures;
            Weights = weights;
        }

        public DotProduct() : base() { }

        public static string GetFragmentShader() {//Texture.SampleLevel(samplerTexture, input.uv, i);
            string textureSample(int i) => $"T{i}.Sample(samplerT{i}, input.uv)";
            var stringBuilder = new System.Text.StringBuilder();

            string MakeIfStatement(int i) {
                return string.Join(" || ", Enumerable.Range(i+1, 16-i).Select(j => $"Count_{j}"));
            }

            stringBuilder.AppendLine($"Texture2D<float4> T0;");
            stringBuilder.AppendLine($"SamplerState samplerT0;");
            for (int i = 1; i < 16; i++) {
                /*
                Texture2D<float4> Texture;
                SamplerState samplerTexture;
                */
                stringBuilder.AppendLine($"#if {MakeIfStatement(i)}");
                stringBuilder.AppendLine($"Texture2D<float4> T{i};");
                stringBuilder.AppendLine($"SamplerState samplerT{i};");
                stringBuilder.AppendLine($"#endif");
            }

            stringBuilder.AppendLine("float4 frag(VSO input) : SV_Target {");

            stringBuilder.AppendLine($"float4 sum = {textureSample(0)} * Weights[0];");

            for (int i = 1; i < 16; i++) {
                stringBuilder.AppendLine($"#if {MakeIfStatement(i)}");
                stringBuilder.AppendLine($"sum += {textureSample(i)} * Weights[{i}];");
                stringBuilder.AppendLine($"#endif");
            }
            //return LoadCode($"{nameof(DotProduct)}.FragmentShaderBody.cginc");
            stringBuilder.AppendLine($"return sum;");
            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }

        protected override void ApplyCustomShaderParameters() {
            base.ApplyCustomShaderParameters();
            for (int i = 0; i < Textures.Count; i++) {
                SetTexture($"T{i}", Textures[i]);
            }
        }

    }


}
