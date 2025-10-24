using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PackPiecewiseLogLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("Threshold", Threshold);
			SetFloat("LeftLogInnerScale", LeftLogInnerScale);
			SetFloat("LeftLogInnerOffset", LeftLogInnerOffset);
			SetFloat("LeftLogOuterScale", LeftLogOuterScale);
			SetFloat("LeftLogOuterOffset", LeftLogOuterOffset);
			SetFloat("RightLogInnerScale", RightLogInnerScale);
			SetFloat("RightLogInnerOffset", RightLogInnerOffset);
			SetFloat("RightLogOuterScale", RightLogOuterScale);
			SetFloat("RightLogOuterOffset", RightLogOuterOffset);
		}
	}
}