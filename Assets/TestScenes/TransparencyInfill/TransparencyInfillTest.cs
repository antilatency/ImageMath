using UnityEngine;
using ImageMath;
using ImageMath.Views;

#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class TransparencyInfillTest : MonoBehaviour{
	[Range(0.0f, 1.0f)]
	public float Power = 0.5f;

	public Vector3 BackupColor = Vector3.zero;
	void Update() {
		/*var descriptor = new RenderTextureDescriptor(128, 128) {
			autoGenerateMips = false,
			useMipMap = true,
			colorFormat = RenderTextureFormat.ARGBFloat,
			enableRandomWrite = true,
            };

		var renderTexture = new RenderTexture(descriptor);
		var compute = Resources.Load<ComputeShader>("GenerateMip");
		int kernel = compute.FindKernel("GenerateMip");

		compute.SetTexture(kernel, "Source", srcTex);
		compute.SetTexture(kernel, "Dest", destTex);*/



		var input = TextureView.GetByName("Input").Texture;
		if (input == null) return;

		var output = TextureView.GetByName("Output").ResizeRenderTexture(input.width, input.height);

		new TransparencyInfill(input) {
			Power = Power,
			BackupColor = BackupColor
		}.PreciseAssignTo(output);

    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}