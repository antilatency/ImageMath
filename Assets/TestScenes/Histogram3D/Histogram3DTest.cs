using UnityEngine;
using ImageMath;
using ImageMath.Views;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class Histogram3DTest : MonoBehaviour{

	public int Size = 2;
	public int[]? Result;
	void Update() {
		var input = TextureView.GetByName("Histogram3DInput").ResizeRenderTexture(512, 512);
		/*if (input == null) {
			Debug.LogWarning("Histogram3DInput texture not found");
			return;
		}*/
		new UVFill() {
		}.AssignTo(input);
		new GradientFill {
			ColorB = new Vector4(0, 0, 1, 1),
		}.AddTo(input);

		var histogram = new Histogram3D(input, new Vector3Int(Size, Size, Size));
		Result = histogram.Execute();

		var sum = Result.Sum();
		if (sum != input.width * input.height) {
			Debug.LogError($"Histogram sum {sum} does not match input texture size {input.width * input.height}");
		}
	}

	public bool DrawCubes = true;

#if UNITY_EDITOR
	public void OnDrawGizmos() {
		if (!DrawCubes) {
			return;
		}
		Handles.matrix = transform.localToWorldMatrix;

		if (Result == null) {
			return;
		}
		var size = Size;
		var normalizedResult = Histogram3D.Normalize(Result);
		for (int x = 0; x < size; x++) {
			float nx = x / (float)(size - 1);
			for (int y = 0; y < size; y++) {
				float ny = y / (float)(size - 1);
				for (int z = 0; z < size; z++) {
					float nz = z / (float)(size - 1);
					var index = x + y * size + z * size * size;
					var value = normalizedResult[index];
					if (value <= 0) {
						continue; // Skip zero values
					}
					var position = new Vector3(x, y, z);

					Handles.color = new Color(nx, ny, nz, 0.5f);
					Handles.CubeHandleCap(0, position, Quaternion.identity, value, EventType.Repaint);
				}
			}
		}

		Handles.matrix = Matrix4x4.identity;
	}
#endif
}