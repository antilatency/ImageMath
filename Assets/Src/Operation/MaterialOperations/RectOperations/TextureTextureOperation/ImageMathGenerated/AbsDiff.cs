using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record AbsDiff {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Multiplier", Multiplier);
		}
	}
}