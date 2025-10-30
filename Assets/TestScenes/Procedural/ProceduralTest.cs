using UnityEngine;
using ImageMath;
using ImageMath.Views;
using static ImageMath.Static;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable



[ExecuteAlways]
public class ProceduralTest : MonoBehaviour{

	public Matrix4x4 Matrix = Matrix4x4.identity;

	void Update() {


		var identity = FlatLUT3DView.GetByName("Identity").ResizeLUT(32);
		identity.InitializeIdentity();

		var srgb = FlatLUT3DView.GetByName("sRGB").ResizeLUTRenderable(32);

		using var temp = GetTempRenderTextureLike(identity.Texture);
		new TextureMultipliedByMatrix(identity.Texture, Matrix).AssignTo(temp.Value);

		new PackSRGB(temp.Value).AssignTo(srgb.RenderTexture);


	}

	/*public Material Material;

	

	void OnRenderObject() {



		Material.SetPass(0);

		Camera cam = Camera.current;
		//GL.PushMatrix();

		Matrix4x4 gpuProj = GL.GetGPUProjectionMatrix(cam.projectionMatrix, false);

		Material.SetMatrix("_ObjectToWorld", transform.localToWorldMatrix);
		Material.SetMatrix("_ObjectToClip", gpuProj * cam.worldToCameraMatrix * transform.localToWorldMatrix);

		Graphics.DrawProceduralNow( MeshTopology.Points, 32*32*32);
    }*/

#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;

		Handles.matrix = Matrix4x4.identity;
	}
#endif
}