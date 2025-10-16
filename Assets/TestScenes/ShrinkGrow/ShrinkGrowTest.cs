using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class ShrinkGrowTest : MonoBehaviour{

	void Update() {
		var inputTexture = TextureView.GetByName("Input")?.Texture;
		if (inputTexture == null) {
			Debug.LogError("Input texture is null");
			return;
		}

		var operation = new ShrinkGrow(inputTexture);
		var resultTexture = TextureView.GetByName("Result").ResizeRenderTexture(inputTexture.width, inputTexture.height);
		resultTexture.filterMode = FilterMode.Point;
		operation.AssignTo(resultTexture);
    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}