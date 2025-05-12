using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ConvexHullSelect {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			Shader.SetGlobalVectorArray("ImageMath_A64V0", Planes);
			Shader.SetGlobalInt("ImageMath_A64V0_Size", Planes.Length);
		}
	}
}