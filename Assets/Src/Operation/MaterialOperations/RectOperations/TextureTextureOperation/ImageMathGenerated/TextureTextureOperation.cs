using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record TextureTextureOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetTexture("TextureA", TextureA);
			SetTexture("TextureB", TextureB);
		}
	}
}