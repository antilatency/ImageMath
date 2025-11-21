#nullable enable

using Accord.Statistics.Filters;
using Accord.Statistics.Kernels;

using UnityEngine;

namespace ImageMath {

    public static partial class TransferFunctions {

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
                LogOuterOffset = (float)(c - 6.0 * b / 14.0),
            };
        }

        public static UnpackPiecewiseLinearLog UnpackArriLogC4(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLinearLog)(PackArriLogC4().CreateInverse(texture));
        }

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

        public static PackPiecewiseLogLog PackCanonLog(Texture? texture = null) {

            // https://www.usa.canon.com/content/dam/canon-assets/white-papers/pro/white-paper-canon-log-gamma-curves.pdf
            // Canon Log Gamma Curves: Description of the Canon Log, Canon Log 2 and Canon Log 3 Gamma Curves
            // November 1st, 2018

            const double ln10 = 2.3025850929940456840179914546844;

            // The spec does not contain named constants, presenting only numeric values.
            // See Appendix 1a for the formula.
            return new PackPiecewiseLogLog(texture) {
                Threshold = 0.0f,
                LeftLogInnerScale = -10.1596f,
                LeftLogInnerOffset = 1.0f,
                LeftLogOuterScale = (float)(-0.45310179 / ln10),
                LeftLogOuterOffset = 0.12512248f,
                RightLogInnerScale = 10.1596f,
                RightLogInnerOffset = 1.0f,
                RightLogOuterScale = (float)(0.45310179 / ln10),
                RightLogOuterOffset = 0.12512248f,
            };
        }

        public static UnpackPiecewiseLogLog UnpackCanonLog(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLogLog)(PackCanonLog().CreateInverse(texture));
        }

        public static PackPiecewiseLogLog PackCanonLog2(Texture? texture = null) {

            // https://www.usa.canon.com/content/dam/canon-assets/white-papers/pro/white-paper-canon-log-gamma-curves.pdf
            // Canon Log Gamma Curves: Description of the Canon Log, Canon Log 2 and Canon Log 3 Gamma Curves
            // November 1st, 2018

            const double ln10 = 2.3025850929940456840179914546844;

            // The spec does not contain named constants, presenting only numeric values.
            // See Appendix 2a for the formula.
            return new PackPiecewiseLogLog(texture) {
                Threshold = 0.0f,
                LeftLogInnerScale = -87.099375f,
                LeftLogInnerOffset = 1.0f,
                LeftLogOuterScale = (float)(- 0.24136077 / ln10),
                LeftLogOuterOffset = 0.092864125f,
                RightLogInnerScale = 87.099375f,
                RightLogInnerOffset = 1.0f,
                RightLogOuterScale = (float)(0.24136077 / ln10),
                RightLogOuterOffset = 0.092864125f,
            };
        }

        public static UnpackPiecewiseLogLog UnpackCanonLog2(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLogLog)(PackCanonLog2().CreateInverse(texture));
        }

        public static PackPiecewiseLogLinearLog PackCanonLog3(Texture? texture = null) {

            // https://www.usa.canon.com/content/dam/canon-assets/white-papers/pro/white-paper-canon-log-gamma-curves.pdf
            // Canon Log Gamma Curves: Description of the Canon Log, Canon Log 2 and Canon Log 3 Gamma Curves
            // November 1st, 2018

            const double ln10 = 2.3025850929940456840179914546844;

            // The spec does not contain named constants, presenting only numeric values.
            // See Appendix 3a for the formula.
            return new PackPiecewiseLogLinearLog(texture) {
                LeftThreshold = -0.014f,
                RightThreshold = 0.014f,
                LeftLogInnerScale = -14.98325f,
                LeftLogInnerOffset = 1.0f,
                LeftLogOuterScale = (float)(-0.36726845 / ln10),
                LeftLogOuterOffset = 0.12783901f,
                LinearScale = 1.9754798f,
                LinearOffset = 0.12512219f,
                RightLogInnerScale = 14.98325f,
                RightLogInnerOffset = 1.0f,
                RightLogOuterScale = (float)(0.36726845 / ln10),
                RightLogOuterOffset = 0.12240537f,
            };
        }

        public static UnpackPiecewiseLogLinearLog UnpackCanonLog3(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLogLinearLog)(PackCanonLog3().CreateInverse(texture));
        }

        public static PiecewiseLinearPow PackRec709(Texture? texture = null) {

            // https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf
            // Recommendation ITU-R BT.709-6 (06/2015): Parameter values for the HDTV standards for
            // production and international programme exchange

            // The spec does not contain named constants, presenting only numeric values.
            // See section 1, item 1.2 for the formula.
            return new PiecewiseLinearPow(texture) {
                Threshold = 0.018f,
                LinearScale = 4.5f,
                LinearOffset = 0.0f,
                PowInnerScale = 1.0f,
                PowInnerOffset = 0.0f,
                PowExponent = 0.45f,
                PowOuterScale = 1.099f,
                PowOuterOffset = -0.099f,
            };
        }

        public static PiecewiseLinearPow UnpackRec709(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (PiecewiseLinearPow)(PackRec709().CreateInverse(texture));
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

        public static PackPiecewiseLinearLog PackSonySlog3(Texture? texture = null) {

            // https://pro.sony/s3/cms-static-content/uploadfile/06/1237494271406.pdf
            // Technical Summary for S-Gamut3.Cine/S-Log3 and S-Gamut3/S-Log3

            const double ln10 = 2.3025850929940456840179914546844;

            // The spec does not contain named constants, presenting only numeric values.
            // See Appendix for the formula.
            return new PackPiecewiseLinearLog(texture) {
                Threshold = 0.01125f,
                LinearScale = (float)((171.2102946929 - 95.0) / (0.01125 * 1023.0)),
                LinearOffset = (float)(95.0 / 1023.0),
                LogInnerScale = (float)(1.0 / (0.18 + 0.01)),
                LogInnerOffset = (float)(0.01 / (0.18 + 0.01)),
                LogOuterScale = (float)(261.5 / (1023.0 * ln10)),
                LogOuterOffset = (float)(420.0 / 1023.0),
            };
        }

        public static UnpackPiecewiseLinearLog UnpackSonySlog3(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (UnpackPiecewiseLinearLog)(PackSonySlog3().CreateInverse(texture));
        }

        public static PiecewiseLinearPow PackSrgb(Texture? texture = null) {

            // TODO: add spec later.

            return new PiecewiseLinearPow(texture) {
                Threshold = 0.0031308f,
                LinearScale = 12.92f,
                LinearOffset = 0.0f,
                PowInnerScale = 1.0f,
                PowInnerOffset = 0.0f,
                PowExponent = (float)(1.0 / 2.4),
                PowOuterScale = 1.055f,
                PowOuterOffset = -0.055f,
            };
        }

        public static PiecewiseLinearPow UnpackSrgb(Texture? texture = null) {
            // TODO: get rid of this cast when covariant overrides are available.
            return (PiecewiseLinearPow)(PackSrgb().CreateInverse(texture));
        }
    }
}
