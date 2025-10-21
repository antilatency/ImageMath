#nullable enable

using UnityEngine;

namespace ImageMath {

    public static partial class TransferFunctions {

        private static class BlackmagicDesignFilmGen5 {
            // Constants from the spec with their original names.
            public const double A = 0.08692876065491224;
            public const double B = 0.005494072432257808;
            public const double C = 0.5300133392291939;
            public const double D = 8.283605932402494;
            public const double E = 0.09246575342465753;
            public const double LIN_CUT = 0.005;
        }

        public static PackPiecewiseLinearLog PackBlackmagicDesignFilmGen5(Texture? texture = null) {
            return new PackPiecewiseLinearLog(texture) {
                Threshold = (float)BlackmagicDesignFilmGen5.LIN_CUT,
                LinearScale = (float)BlackmagicDesignFilmGen5.D,
                LinearOffset = (float)BlackmagicDesignFilmGen5.E,
                LogInnerScale = 1.0f,
                LogInnerOffset = (float)BlackmagicDesignFilmGen5.B,
                LogOuterScale = (float)BlackmagicDesignFilmGen5.A,
                LogOuterOffset = (float)BlackmagicDesignFilmGen5.C,
            };
        }

        public static UnpackPiecewiseLinearLog UnpackBlackmagicDesignFilmGen5(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLinearLog)(PackBlackmagicDesignFilmGen5().CreateInverse(texture));
        }
    }
}
