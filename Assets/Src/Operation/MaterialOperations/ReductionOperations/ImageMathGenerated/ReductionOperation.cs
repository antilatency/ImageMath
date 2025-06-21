using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ReductionOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
		}
	}
}