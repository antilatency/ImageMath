using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureMultiplyAdd {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetMatrix("Multiplier", Multiplier);
			SetVector("Increment", Increment);
		}
	}
}