using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ClippingDetection {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
		}
	}
}