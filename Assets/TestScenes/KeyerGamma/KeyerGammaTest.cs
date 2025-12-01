using UnityEngine;
using ImageMath;
using ImageMath.Views;
using static ImageMath.Static;
#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class KeyerGammaTest : MonoBehaviour{
	[Range(0.5f, 2f)]
	public float Exponent = 1;
	void Update() {

		var Output = TextureView.GetByName("ImageA").ResizeRenderTexture(1920, 1080);
		using var a = GetTempRenderTexture(1920, 1080);


		new GradientFill(new Vector4(1, 0, 0, 1), new Vector4(1, 1, 1, 1), null, null) {

		}.AssignTo(a);

		new GradientFill(new Vector4(1, 0, 0, 1), new Vector4(0, 1, 0, 1), null, null) {
			Size = new Vector2(1, 0.5f)
		}.AssignTo(a);

		new TexturePow(a, Exponent).AssignTo(Output);

    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}