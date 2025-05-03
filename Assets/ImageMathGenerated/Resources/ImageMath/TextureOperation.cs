using UnityEngine;
namespace ImageMath {
	public partial record TextureOperation {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalTexture("ImageMath_T0", Texture);
		}
	}
}
