using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackPiecewiseLinearLog {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetFloat("InvThreshold", InvThreshold);
			SetFloat("InvLinearScale", InvLinearScale);
			SetFloat("InvLinearOffset", InvLinearOffset);
			SetFloat("InvExpInnerScale", InvExpInnerScale);
			SetFloat("InvExpInnerOffset", InvExpInnerOffset);
			SetFloat("InvExpOuterScale", InvExpOuterScale);
			SetFloat("InvExpOuterOffset", InvExpOuterOffset);
		}
	}
}