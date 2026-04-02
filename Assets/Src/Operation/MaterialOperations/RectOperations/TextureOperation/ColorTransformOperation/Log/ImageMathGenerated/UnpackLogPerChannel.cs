using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record UnpackLogPerChannel {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("WhiteLevel", WhiteLevel);
			SetVector("BlackLevel", BlackLevel);
			SetVector("ExponentScale", ExponentScale);
		}
	}
}