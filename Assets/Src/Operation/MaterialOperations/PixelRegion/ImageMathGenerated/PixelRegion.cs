using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PixelRegion {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
			ShaderSetGlobalUtils.SetGlobalVectorInt("Offset", Offset);
			if (ClampPixelCoordinates) EnableKeyword("ClampPixelCoordinates"); else DisableKeyword("ClampPixelCoordinates");
		}
	}
}