using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureRGBMultipliedByMatrix {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetMatrix("Matrix", Matrix);
		}
	}
}