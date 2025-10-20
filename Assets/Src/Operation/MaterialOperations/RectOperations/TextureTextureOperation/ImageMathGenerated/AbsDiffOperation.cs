using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record AbsDiffOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Multiplier", Multiplier);
		}
	}
}