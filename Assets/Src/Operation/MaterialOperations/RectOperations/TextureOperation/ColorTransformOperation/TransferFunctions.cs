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

        public static PackPiecewiseLinearLog PackArriLogC3(Texture? texture = null) {

            // https://www.arri.com/resource/blob/31918/66f56e6abb6e5b6553929edf9aa7483e/2017-03-alexa-logc-curve-in-vfx-data.pdf
            // ALEXA Log C Curve: Usage in VFX, by Harald Brendel, 09-Mar-17

            // Constants from the spec with their original names, for the "default" curve (EI 800). Search
            // the document for "Use the parameters in the following table for conversion between Log C values
            // and exposure values" and refer to the table row with the EI value of 800.
            const double cut = 0.010591;
            const double a = 5.555556;
            const double b = 0.052272;
            const double c = 0.247190;
            const double d = 0.385537;
            const double e = 5.367655;
            const double f = 0.092809;

            const double ln10 = 2.3025850929940456840179914546844;

            return new PackPiecewiseLinearLog(texture) {
                Threshold = (float)cut,
                LinearScale = (float)e,
                LinearOffset = (float)f,
                LogInnerScale = (float)a,
                LogInnerOffset = (float)b,
                LogOuterScale = (float)(c / ln10),
                LogOuterOffset = (float)d,
            };
        }

        public static UnpackPiecewiseLinearLog UnpackArriLogC3(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLinearLog)(PackArriLogC3().CreateInverse(texture));
        }
    }
}
