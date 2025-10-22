using UnityEngine;
using ImageMath;
using ImageMath.Views;
using System.Linq;

#nullable enable

[ExecuteAlways]
public class LogTest : MonoBehaviour{

	public float DiffMultiplier = 1.0f;

	void Update() {

		var bdfg5 = TextureView.GetByName("bdfg5").Texture;


		var linear = TextureView.GetByName("Gradient").ResizeRenderTexture(2048, 256);

		new GradientFill() {
			PointA = new Vector2(0.5f / 2048, 0),
			PointB = new Vector2(1 - 0.5f / 2048, 0),
		}.AssignTo(linear);

		var unpackOperation = TransferFunctions.UnpackBlackmagicDesignFilmGen5(linear);
		var packOperation = TransferFunctions.PackBlackmagicDesignFilmGen5();

		var unpacked = TextureView.GetByName("Unpacked").ResizeRenderTexture(linear.width, linear.height);
		unpackOperation.AssignTo(unpacked);

		var diff = TextureView.GetByName("Diff").ResizeRenderTexture(linear.width, linear.height);

		new AbsDiffOperation(bdfg5, unpacked, DiffMultiplier).AssignTo(diff);


		var equal = TextureView.GetByName("Equal").ResizeRenderTexture(linear.width, linear.height);
		new TextureCompare {
			Texture = diff,
			Operation = TextureCompare.CompareOperation.Equal,
			Reference = new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
		}.AssignTo(equal);

		var roundTrip = TextureView.GetByName("RoundTrip").ResizeRenderTexture(linear.width, linear.height);
		TransferFunctions.PackBlackmagicDesignFilmGen5(unpacked).AssignTo(roundTrip);

		var roundTripDiff = TextureView.GetByName("RoundTripDiff").ResizeRenderTexture(linear.width, linear.height);
		new AbsDiffOperation(linear, roundTrip, DiffMultiplier).AssignTo(roundTripDiff);
    }
}
