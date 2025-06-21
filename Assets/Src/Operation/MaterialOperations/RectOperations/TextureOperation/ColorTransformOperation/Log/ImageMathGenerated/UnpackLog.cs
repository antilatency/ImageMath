using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("BlackLevel", BlackLevel);
			SetFloat("ExponentScale", ExponentScale);
			SetFloat("Multiplier", Multiplier);
		}
	}
}