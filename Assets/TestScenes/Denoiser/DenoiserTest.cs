using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class DenoiserTest : MonoBehaviour{
	[Range(0, 1f)]
	public float Power = 1.0f;

	public Denoiser.KernelSize Size = Denoiser.KernelSize._5x5;

	public bool RenderDelta = false;

	float P4(float x) {
		float x2 = x * x;
		float x4 = x2 * x2;
		return x4;
	}

	void Update() {
		var input = TextureView.GetByName("Input").Texture;
		if (input == null) return;


		var output = TextureView.GetByName("Output").ResizeRenderTexture(input.width, input.height);
		new Denoiser(input) {
			Level = P4(Power),
			Size = Size,
			RenderDelta = RenderDelta
		}
		.AssignTo(output);

	}

#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;

		Handles.matrix = Matrix4x4.identity;
	}
#endif
}