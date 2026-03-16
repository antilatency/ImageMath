using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record GaussianNoiseFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("Amplitude", Amplitude);
		}
	}
}