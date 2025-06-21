using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ColorFill {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Color", Color);
		}
	}
}