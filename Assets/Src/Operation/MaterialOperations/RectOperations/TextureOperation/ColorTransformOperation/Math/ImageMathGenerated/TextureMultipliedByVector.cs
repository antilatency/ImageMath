using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureMultipliedByVector {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Multiplier", Multiplier);
		}
	}
}