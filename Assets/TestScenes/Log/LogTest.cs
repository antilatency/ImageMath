#nullable enable
using ImageMath;
using ImageMath.Views;

using UnityEngine;

[ExecuteAlways]
public class LogTest : MonoBehaviour {

    public enum TransferFunction {
        BlackmagicDesignFilmGen5,
        ArriLogC3,
    }

    public TransferFunction transferFunction;

    public float DiffMultiplier = 1.0f;

    void Update() {

        ColorTransformOperation packOperation(Texture? texture = null) {
            switch (transferFunction) {
                case TransferFunction.BlackmagicDesignFilmGen5:
                    return TransferFunctions.PackBlackmagicDesignFilmGen5(texture);
                case TransferFunction.ArriLogC3:
                    return TransferFunctions.PackArriLogC3(texture);
            }

            throw new System.ArgumentException();
        }

        ColorTransformOperation unpackOperation(Texture? texture = null) {
            switch (transferFunction) {
                case TransferFunction.BlackmagicDesignFilmGen5:
                    return TransferFunctions.UnpackBlackmagicDesignFilmGen5(texture);
                case TransferFunction.ArriLogC3:
                    return TransferFunctions.UnpackArriLogC3(texture);
            }

            throw new System.ArgumentException();
        }

        var bdfg5 = TextureView.GetByName("bdfg5").Texture;

        var linear = TextureView.GetByName("Gradient").ResizeRenderTexture(2048, 256);

        new GradientFill() {
            PointA = new Vector2(0.5f / 2048, 0),
            PointB = new Vector2(1 - 0.5f / 2048, 0),
        }.AssignTo(linear);

        var unpacked = TextureView.GetByName("Unpacked").ResizeRenderTexture(linear.width, linear.height);
        unpackOperation(linear).AssignTo(unpacked);

        var diff = TextureView.GetByName("Diff").ResizeRenderTexture(linear.width, linear.height);

        new AbsDiffOperation(bdfg5, unpacked, DiffMultiplier).AssignTo(diff);

        var equal = TextureView.GetByName("Equal").ResizeRenderTexture(linear.width, linear.height);
        new TextureCompare {
            Texture = diff,
            Operation = TextureCompare.CompareOperation.Equal,
            Reference = new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
        }.AssignTo(equal);

        var roundTrip = TextureView.GetByName("RoundTrip").ResizeRenderTexture(linear.width, linear.height);
        packOperation(unpacked).AssignTo(roundTrip);

        var roundTripDiff = TextureView.GetByName("RoundTripDiff").ResizeRenderTexture(linear.width, linear.height);
        new AbsDiffOperation(linear, roundTrip, DiffMultiplier).AssignTo(roundTripDiff);
    }
}
