using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("Texture", Texture);
		}
	}
}