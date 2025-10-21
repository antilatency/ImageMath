using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PackPiecewiseLinearLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("Threshold", Threshold);
			SetFloat("LinearScale", LinearScale);
			SetFloat("LinearOffset", LinearOffset);
			SetFloat("LogInnerScale", LogInnerScale);
			SetFloat("LogInnerOffset", LogInnerOffset);
			SetFloat("LogOuterScale", LogOuterScale);
			SetFloat("LogOuterOffset", LogOuterOffset);
		}
	}
}