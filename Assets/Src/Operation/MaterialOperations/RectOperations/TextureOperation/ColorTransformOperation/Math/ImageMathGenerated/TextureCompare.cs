using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureCompare {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Reference", Reference);
			SetInt("EqualOperation", EqualOperation);
			SetFloat("PreMultiplier", PreMultiplier);
			SetInt("PostInverse", PostInverse);
		}
	}
}