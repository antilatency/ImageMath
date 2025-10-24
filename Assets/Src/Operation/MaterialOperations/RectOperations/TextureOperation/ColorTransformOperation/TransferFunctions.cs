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

        public static PackPiecewiseLinearLog PackArriLogC4(Texture? texture = null) {

            // https://www.arri.com/resource/blob/278790/bea879ac0d041a925bed27a096ab3ec2/2022-05-arri-logc4-specification-data.pdf
            // ARRI LogC4: Logarithmic Color Specification, by Sean Cooper, 2025-01-23

            const double ln2 = 0.69314718055994530941723212145818;

            // Constants from the spec with their original names. Hoewever, the spec does not
            // provide their numeric values directly, presenting a set of bizarre expressions
            // instead. See Appendix A, "Reference CTL Implementation".
            const double a = 2231.8263090676883;
            const double b = 0.9071358748778103;
            const double c = 0.09286412512218964;
            const double s = 0.1135972086105891;
            const double t = -0.01805699611991131;

            return new PackPiecewiseLinearLog(texture) {
                Threshold = (float)t,
                LinearScale = (float)(1.0 / s),
                LinearOffset = (float)(-t / s),
                LogInnerScale = (float)a,
                LogInnerOffset = 64.0f,
                LogOuterScale = (float)(b / (14.0 * ln2)),
                LogOuterOffset = (float)(c - 6.0*b/14.0),
            };
        }

        public static UnpackPiecewiseLinearLog UnpackArriLogC4(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLinearLog)(PackArriLogC4().CreateInverse(texture));
        }

        public static PackPiecewiseLinearLog PackRedLog3G10(Texture? texture = null) {

            // https://docs.red.com/955-0187/PDF/915-0187%20Rev-C%20%20%20RED%20OPS%2C%20White%20Paper%20on%20REDWideGamutRGB%20and%20Log3G10.pdf
            // White Paper on REDWideGamutRGB and Log3G10

            // Constants from the spec with their original names and values. See the reference
            // implementation provided in the "Equations" section, page 4.
            const double a = 0.224282;
            const double b = 155.975327;
            const double c = 0.01;
            const double g = 15.1927;

            const double ln10 = 2.3025850929940456840179914546844;

            return new PackPiecewiseLinearLog(texture) {
                Threshold = (float)(-c),
                LinearScale = (float)g,
                LinearOffset = (float)(c*g),
                LogInnerScale = (float)b,
                LogInnerOffset = (float)(b*c + 1.0),
                LogOuterScale = (float)(a / ln10),
                LogOuterOffset = 0.0f,
            };
        }

        public static UnpackPiecewiseLinearLog UnpackRedLog3G10(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLinearLog)(PackRedLog3G10().CreateInverse(texture));
        }
    }
}
