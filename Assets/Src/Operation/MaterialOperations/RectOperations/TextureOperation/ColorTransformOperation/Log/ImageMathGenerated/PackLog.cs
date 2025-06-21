using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PackLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("BlackLevel", BlackLevel);
			SetFloat("InverseExponentScale", InverseExponentScale);
			SetFloat("Multiplier", Multiplier);
		}
	}
}