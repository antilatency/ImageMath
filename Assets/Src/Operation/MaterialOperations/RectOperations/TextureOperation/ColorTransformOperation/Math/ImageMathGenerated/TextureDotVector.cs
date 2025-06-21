using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureDotVector {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVector("Vector", Vector);
		}
	}
}