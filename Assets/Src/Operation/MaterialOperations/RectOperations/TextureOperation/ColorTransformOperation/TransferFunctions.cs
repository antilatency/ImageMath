#nullable enable

using UnityEngine;

namespace ImageMath {

    public static partial class TransferFunctions {

        public static PackPiecewiseLinearLog PackBlackmagicDesignFilmGen5(Texture? texture = null) {

            // Constants from the spec with their original names.
            const double A = 0.08692876065491224;
            const double B = 0.005494072432257808;
            const double C = 0.5300133392291939;
            const double D = 8.283605932402494;
            const double E = 0.09246575342465753;
            const double LIN_CUT = 0.005;

            return new PackPiecewiseLinearLog(texture) {
                Threshold = (float)LIN_CUT,
                LinearScale = (float)D,
                LinearOffset = (float)E,
                LogInnerScale = 1.0f,
                LogInnerOffset = (float)B,
                LogOuterScale = (float)A,
                LogOuterOffset = (float)C,
            };
        }

        public static UnpackPiecewiseLinearLog UnpackBlackmagicDesignFilmGen5(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLinearLog)(PackBlackmagicDesignFilmGen5().CreateInverse(texture));
        }
    }
}
