using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class LogTest : MonoBehaviour{

	public float ExponentScale = 9.0f;
	public float BlackLevel = 0.0f;
	public float WhiteLevel = 1.0f;

	public Vector3 TestValue = new Vector3(0.2f, 0.4f, 0.6f);
	public Vector3 Packed;
	public Vector3 Unpacked;

	void Update() {
		var input = TextureView.GetByName("Input").ResizeRenderTexture(256, 256);
		new GradientFill().AssignTo(input);

		var packOperation = new PackLog {
			Texture = input,
			ExponentScale = ExponentScale * Vector3.one,
			BlackLevel = BlackLevel * Vector3.one,
			WhiteLevel = WhiteLevel * Vector3.one,
		};

		

		var log = TextureView.GetByName("Log").ResizeRenderTexture(256, 256);
		packOperation.AssignTo(log);

		var unpackOperation = new UnpackLog {
			Texture = log,
			ExponentScale = ExponentScale * Vector3.one,
			BlackLevel = BlackLevel * Vector3.one,
			WhiteLevel = WhiteLevel * Vector3.one,
		};

		var unpacked = TextureView.GetByName("Unpacked").ResizeRenderTexture(256, 256);
		
		unpackOperation.AssignTo(unpacked);

		var diff = TextureView.GetByName("Diff").ResizeRenderTexture(256, 256);
		diff.Assign(input);
		diff.SubstructRGB(unpacked);


		var equal = TextureView.GetByName("Equal").ResizeRenderTexture(256, 256);
		new TextureCompare {
			Texture = diff,
			Operation = TextureCompare.CompareOperation.Equal,
			Reference = new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
		}.AssignTo(equal);

		Packed = packOperation.Convert(TestValue);
		Unpacked = unpackOperation.Convert(Packed);


    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}