#nullable enable
using ImageMath;
using ImageMath.Views;

using UnityEngine;

[ExecuteAlways]
public class LogTest : MonoBehaviour {

    public enum TransferFunction {
        ArriLogC3,
        ArriLogC4,
        CanonLog,
        CanonLog2,
        CanonLog3,
        BlackmagicDesignFilmGen5,
        Rec709,
        RedLog3G10,
        SonySlog3,
        Srgb,
    }

    public TransferFunction transferFunction;

    public float DiffMultiplier = 1.0f;

    void Update() {

        ColorTransformOperation packOperation(Texture? texture = null) {
            switch (transferFunction) {
                case TransferFunction.ArriLogC3:
                    return TransferFunctions.PackArriLogC3(texture);
                case TransferFunction.ArriLogC4:
                    return TransferFunctions.PackArriLogC4(texture);
                case TransferFunction.BlackmagicDesignFilmGen5:
                    return TransferFunctions.PackBlackmagicDesignFilmGen5(texture);
                case TransferFunction.CanonLog:
                    return TransferFunctions.PackCanonLog(texture);
                case TransferFunction.CanonLog2:
                    return TransferFunctions.PackCanonLog2(texture);
                case TransferFunction.CanonLog3:
                    return TransferFunctions.PackCanonLog3(texture);
                case TransferFunction.Rec709:
                    return TransferFunctions.PackRec709(texture);
                case TransferFunction.RedLog3G10:
                    return TransferFunctions.PackRedLog3G10(texture);
                case TransferFunction.SonySlog3:
                    return TransferFunctions.PackSonySlog3(texture);
                case TransferFunction.Srgb:
                    return TransferFunctions.PackSRGB(texture);
            }

            throw new System.ArgumentException();
        }

        ColorTransformOperation unpackOperation(Texture? texture = null) {
            switch (transferFunction) {
                case TransferFunction.ArriLogC3:
                    return TransferFunctions.UnpackArriLogC3(texture);
                case TransferFunction.ArriLogC4:
                    return TransferFunctions.UnpackArriLogC4(texture);
                case TransferFunction.BlackmagicDesignFilmGen5:
                    return TransferFunctions.UnpackBlackmagicDesignFilmGen5(texture);
                case TransferFunction.CanonLog:
                    return TransferFunctions.UnpackCanonLog(texture);
                case TransferFunction.CanonLog2:
                    return TransferFunctions.UnpackCanonLog2(texture);
                case TransferFunction.CanonLog3:
                    return TransferFunctions.UnpackCanonLog3(texture);
                case TransferFunction.Rec709:
                    return TransferFunctions.UnpackRec709(texture);
                case TransferFunction.RedLog3G10:
                    return TransferFunctions.UnpackRedLog3G10(texture);
                case TransferFunction.SonySlog3:
                    return TransferFunctions.UnpackSonySlog3(texture);
                case TransferFunction.Srgb:
                    return TransferFunctions.UnpackSRGB(texture);
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

        new AbsDiff(bdfg5, unpacked, DiffMultiplier).AssignTo(diff);

        var equal = TextureView.GetByName("Equal").ResizeRenderTexture(linear.width, linear.height);
        new TextureCompare {
            Texture = diff,
            Operation = TextureCompare.CompareOperation.Equal,
            Reference = new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
        }.AssignTo(equal);

        var roundTrip = TextureView.GetByName("RoundTrip").ResizeRenderTexture(linear.width, linear.height);
        packOperation(unpacked).AssignTo(roundTrip);

        var roundTripDiff = TextureView.GetByName("RoundTripDiff").ResizeRenderTexture(linear.width, linear.height);
        new AbsDiff(linear, roundTrip, DiffMultiplier).AssignTo(roundTripDiff);
    }
}
