using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackPiecewiseLogLinearLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("InvLeftThreshold", InvLeftThreshold);
			SetFloat("InvRightThreshold", InvRightThreshold);
			SetFloat("InvLinearScale", InvLinearScale);
			SetFloat("InvLinearOffset", InvLinearOffset);
			SetFloat("InvLeftExpInnerScale", InvLeftExpInnerScale);
			SetFloat("InvLeftExpInnerOffset", InvLeftExpInnerOffset);
			SetFloat("InvLeftExpOuterScale", InvLeftExpOuterScale);
			SetFloat("InvLeftExpOuterOffset", InvLeftExpOuterOffset);
			SetFloat("InvRightExpInnerScale", InvRightExpInnerScale);
			SetFloat("InvRightExpInnerOffset", InvRightExpInnerOffset);
			SetFloat("InvRightExpOuterScale", InvRightExpOuterScale);
			SetFloat("InvRightExpOuterOffset", InvRightExpOuterOffset);
		}
	}
}