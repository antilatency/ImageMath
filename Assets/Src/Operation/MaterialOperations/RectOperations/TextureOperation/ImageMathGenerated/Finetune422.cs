using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record Finetune422 {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("YAxisInRGBSpace", YAxisInRGBSpace);
			SetTexture("LinearTexture", LinearTexture);
			SetTexture("GammaTexture", GammaTexture);
		}
	}
}