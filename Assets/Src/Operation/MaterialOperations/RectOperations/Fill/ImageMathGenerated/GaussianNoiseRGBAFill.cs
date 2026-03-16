using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record GaussianNoiseRGBAFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Amplitude", Amplitude);
		}
	}
}