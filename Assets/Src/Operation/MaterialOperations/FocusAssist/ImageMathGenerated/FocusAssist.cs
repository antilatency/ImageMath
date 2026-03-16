using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record FocusAssist {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
			SetFloat("Multiplier", Multiplier);
		}
	}
}