using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record PackPiecewiseLogLinearLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("LeftThreshold", LeftThreshold);
			SetFloat("RightThreshold", RightThreshold);
			SetFloat("LeftLogInnerScale", LeftLogInnerScale);
			SetFloat("LeftLogInnerOffset", LeftLogInnerOffset);
			SetFloat("LeftLogOuterScale", LeftLogOuterScale);
			SetFloat("LeftLogOuterOffset", LeftLogOuterOffset);
			SetFloat("LinearScale", LinearScale);
			SetFloat("LinearOffset", LinearOffset);
			SetFloat("RightLogInnerScale", RightLogInnerScale);
			SetFloat("RightLogInnerOffset", RightLogInnerOffset);
			SetFloat("RightLogOuterScale", RightLogOuterScale);
			SetFloat("RightLogOuterOffset", RightLogOuterOffset);
		}
	}
}