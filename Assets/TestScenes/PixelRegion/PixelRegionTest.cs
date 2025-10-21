using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class PixelRegionTest : MonoBehaviour{

	public Vector2Int Offset = Vector2Int.zero;
	public int MipLevel = 0;

	public bool ClampPixelCoordinates = false;

	void Update() {
		var inputTexture = TextureView.GetByName("Input").ResizeRenderTexture(512, 512);
		inputTexture.filterMode = FilterMode.Point;
		new UVFill().AssignTo(inputTexture);

		var pixelRegionTexture = TextureView.GetByName("PixelRegion").ResizeRenderTexture(32, 32);
		pixelRegionTexture.filterMode = FilterMode.Point;
		new PixelRegion(inputTexture, Offset, MipLevel){ ClampPixelCoordinates = ClampPixelCoordinates }.AssignTo(pixelRegionTexture);
    }

#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;

		Handles.matrix = Matrix4x4.identity;
	}
#endif
}