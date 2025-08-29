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
	public string VideoFilePath;

	[Multiline(20)]
	public string VideoInfoJson;

	public void OpenVideo() {
#if UNITY_EDITOR
		VideoFilePath = EditorUtility.OpenFilePanel("Open Video", "", string.Join(",", FFMPEG.SupportedFormats));
		if (string.IsNullOrEmpty(VideoFilePath)) return;

		VideoInfoJson = FFProbe.GetJson(VideoFilePath);
#endif
	}

	public bool _16bit = false;

	[Range(0,1)]
	public float Seek;
	public InspectorButton _ReadVideo;
	public void ReadVideo() {
		int numFramesFact = 0;
		var stopwatch = System.Diagnostics.Stopwatch.StartNew();
		using var reader = new FFMPEGImageReader(VideoFilePath,_16bit ? FFMPEGImageReader.OutputFormat.RGBA64 : FFMPEGImageReader.OutputFormat.RGB24, Debug.Log);
		//reader.Run(192, 108, FFMPEGImageReader.ScaleFlags.Lanczos, reader.CreateSelectFilter(0..4));
		reader.Run(new FFMPEGImageReader.RunParameters {
			InputSeek = Seek*reader.Duration,
			AccurateInputSeek = false,
			OutputWidth = reader.InputWidth / 10,
			OutputHeight = reader.InputHeight / 10,
			ScaleFlags = FFMPEGImageReader.ScaleFlags.Lanczos,
			NumberOfFrames = 1
		});
		var texture = TextureView.GetByName("Frame").ResizeTexture2D(reader.OutputWidth, reader.OutputHeight, false, reader.TextureFormat);
		Debug.Log($"Warmup time: {stopwatch.ElapsedMilliseconds}ms");
		stopwatch.Restart();

		while (!reader.Finished) {
			if (reader.ReadFrame()) {
				texture.SetPixelData(reader.FrameBuffer, 0, 0);
				texture.Apply(false, false);
				numFramesFact++;
			}
		}

		var time = stopwatch.ElapsedMilliseconds;
		float timeS = time / 1000f;
		var numFramesPlan = reader.NumberOfFrames;
		Debug.Log($"Read {numFramesFact} frames in {time}ms, planned {numFramesPlan} frames. {(numFramesFact / timeS)} of {reader.FPS} fps. speed {reader.Duration / timeS}x");
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
		/*if (!string.IsNullOrEmpty(YUVFilePath)) {
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
		*/
			
    }
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}