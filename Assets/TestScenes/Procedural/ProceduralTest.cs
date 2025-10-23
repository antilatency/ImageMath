using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class ProceduralTest : MonoBehaviour{

	public Material Material;

	

	void OnRenderObject() {
		Material.SetPass(0);

		Camera cam = Camera.current;
		//GL.PushMatrix();

		Matrix4x4 gpuProj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);

		Material.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
		Material.SetMatrix("_ObjectToClip", gpuProj * cam.worldToCameraMatrix * transform.localToWorldMatrix);

		Graphics.DrawProceduralNow( MeshTopology.Points, 32*32*32);
    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}