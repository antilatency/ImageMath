using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackPiecewiseLogLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("InvThreshold", InvThreshold);
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