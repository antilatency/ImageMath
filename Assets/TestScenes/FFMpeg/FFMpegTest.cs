using UnityEngine;
using ImageMath;
using ImageMath.Views;
using System.IO;


#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable

[ExecuteAlways]
public class FFMpegTest : MonoBehaviour{


	public InspectorButton _OpenVideo;

	[Multiline(20)]
	public string VideoInfoJson;

	public void OpenVideo() {
#if UNITY_EDITOR
		var filePath = EditorUtility.OpenFilePanel("Open Video", "", string.Join(",", FFMPEG.SupportedFormats));
		if (string.IsNullOrEmpty(filePath)) return;

		VideoInfoJson = FFProbe.GetJson(filePath);
#endif
	}

	public InspectorButton _SelectYUV_FHD;
	public string YUVFilePath;
	public void SelectYUV_FHD() {
#if UNITY_EDITOR
		var filePath = EditorUtility.OpenFilePanel("Open YUV FHD", "", "yuv");
		if (string.IsNullOrEmpty(filePath)) return;
		YUVFilePath = filePath;
		

#endif
	}

	public Texture2D? YTexture;
	public Texture2D? UTexture;
	public Texture2D? VTexture;

	void Update() {
		if (!string.IsNullOrEmpty(YUVFilePath)) {
			var data = File.ReadAllBytes(YUVFilePath);
			if (YTexture == null) {
				YTexture = new Texture2D(1920, 1080, TextureFormat.R16, false, true);
			}
			if (UTexture == null) {
				UTexture = new Texture2D(960, 1080, TextureFormat.R16, false, true);
			}
			if (VTexture == null) {
				VTexture = new Texture2D(960, 1080, TextureFormat.R16, false, true);
			}

			/*//test clear data with 0.5
			for (int i = 0; i < data.Length; i++) {
				data[i] = 0x80; // 0.5 in R16 format
			}*/

			YTexture.SetPixelData(data, 0, 0);
			YTexture.Apply(false, false);
			UTexture.SetPixelData(data, 0, 1920 * 1080 * 2);
			UTexture.Apply(false, false);
			VTexture.SetPixelData(data, 0, 1920 * 1080 * 2 + 960 * 1080 * 2);
			VTexture.Apply(false, false);

		}


		var result = TextureView.GetByName("UnpackYUVResult").ResizeRenderTexture(1920, 1080);
		new UnpackYUV(YTexture, UTexture, VTexture) {
		}.AssignTo(result);


		var resultUnpackedFromSRGB = TextureView.GetByName("UnpackYUVResultFromSRGB").ResizeRenderTexture(1920, 1080);
		new UnpackSRGB(result).FlipY().AssignTo(resultUnpackedFromSRGB);
			
    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}