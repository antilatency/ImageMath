using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Histogram3D {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
			ShaderSetGlobalUtils.SetGlobalVectorInt("Size", Size);
		}
	}
}