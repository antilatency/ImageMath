using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class DenoiserTest : MonoBehaviour{
	[Range(0.0f, 1.0f)]
	public float Power = 1.0f;

	[Range(1, 4)]
	public int Size = 2;

	void Update() {
		var input = TextureView.GetByName("Input").Texture;
		if (input == null) return;


		var output = TextureView.GetByName("Output").ResizeRenderTexture(input.width, input.height);
		new Denoiser(input) {
			Power = Power,
			Size = Size
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