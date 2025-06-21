using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

public enum ColorEnum {
	Red,
	Green,
	Blue,
}

[ExecuteAlways]
public class MulticompileOptionsTest : MonoBehaviour{
	public ColorEnum Color = ColorEnum.Red;
	public bool DrawCircle = true;
    void Update() {

		var renderTarget = TextureView.GetByName("Main").ResizeRenderTexture(512, 512);
		new MulticompileOptionsTestOperation {
			Color = Color,
			DrawCircle = DrawCircle,
		}.AssignTo(renderTarget);

		Debug.Log(Color.ToString());
    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}