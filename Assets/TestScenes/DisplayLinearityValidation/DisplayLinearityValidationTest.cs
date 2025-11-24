using UnityEngine;
using ImageMath;
using ImageMath.Views;
using UnityEngine.Rendering;


#if UNITY_EDITOR
using UnityEditor;
#endif
#nullable enable



[ExecuteAlways]
public class DisplayLinearityValidationTest : MonoBehaviour{

	void Update() {

		var renderTexture = ImageMath.Views.TextureView.GetByName("Main").ResizeRenderTexture(512, 512);
		new DisplayLinearityValidation().AssignTo(renderTexture);



		var sRGBTexture = ImageMath.Views.TextureView.GetByName("sRGB_DisplayLinearityValidationPattern").ResizeRenderTexture(512, 512);
		TransferFunctions.PackSRGB(renderTexture).AssignTo(sRGBTexture);
		
		var rec709Texture = ImageMath.Views.TextureView.GetByName("Rec709_DisplayLinearityValidationPattern").ResizeRenderTexture(512, 512);
		TransferFunctions.PackRec709(renderTexture).AssignTo(rec709Texture);
		//new PackSRGB(renderTexture)

		/*var rec709Texture = ImageMath.Views.TextureView.GetByName("Rec709_DisplayLinearityValidationPattern").ResizeRenderTexture(512, 512);
		new PackRec709(renderTexture).AssignTo(rec709Texture);*/
	}
	
#if UNITY_EDITOR
	public void OnDrawGizmos() {
		Handles.matrix = transform.localToWorldMatrix;
		
		Handles.matrix = Matrix4x4.identity;		
	}
#endif
}