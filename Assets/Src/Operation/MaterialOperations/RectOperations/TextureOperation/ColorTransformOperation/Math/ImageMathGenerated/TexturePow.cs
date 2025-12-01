using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TexturePow {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Exponent", Exponent);
		}
	}
}