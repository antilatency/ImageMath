using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record LambertianSphereFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Color", Color);
			SetVector("LightDirection", LightDirection);
		}
	}
}