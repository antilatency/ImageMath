using UnityEngine;
namespace ImageMath {
	[ImageMath.Generated]
	public partial record ConvexHullSelect {
		protected override void ApplyShaderParameters() {
			base.ApplyShaderParameters();
			SetVectorArray("Planes", ExpandArray(Planes,64));
			Shader.SetGlobalInt("Planes_Size", Planes.Length);
		}
	}
}