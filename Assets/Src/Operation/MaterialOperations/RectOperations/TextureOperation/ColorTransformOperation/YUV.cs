#nullable enable

using UnityEngine;

namespace ImageMath {
    public static class YUV {
        public enum Standard {
            BT601,
            BT709,
            BT2020,
        }

        public static readonly Matrix4x4 BT601_YCbCrFromRGB = new Matrix4x4(
            new Vector4(0.299000f, -0.213267f, 0.395598f, 0.000000f),
            new Vector4(0.587000f, -0.418688f, -0.331264f, 0.000000f),
            new Vector4(0.114000f, 0.631954f, -0.064334f, 0.000000f),
            new Vector4(0.000000f, 0.500000f, 0.500000f, 1.000000f)
        );

        public static readonly Matrix4x4 BT601_RGBFromYCbCr = new Matrix4x4(
            new Vector4(1.000000f, 1.000000f, 1.000000f, 0.000000f),
            new Vector4(-0.000000f, -0.272279f, 1.402000f, 0.000000f),
            new Vector4(1.772000f, -0.902603f, -0.000000f, 0.000000f),
            new Vector4(-0.886000f, 0.587441f, -0.701000f, 1.000000f)
        );

        public static readonly Matrix4x4 BT709_YCbCrFromRGB = new Matrix4x4(
            new Vector4(0.212600f, -0.114572f, 0.500000f, 0.000000f),
            new Vector4(0.715200f, -0.385428f, -0.454153f, 0.000000f),
            new Vector4(0.072200f, 0.500000f, -0.045847f, 0.000000f),
            new Vector4(0.000000f, 0.500000f, 0.500000f, 1.000000f)
        );

        public static readonly Matrix4x4 BT709_RGBFromYCbCr = new Matrix4x4(
            new Vector4(1.000000f, 1.000000f, 1.000000f, 0.000000f),
            new Vector4(-0.000000f, -0.187324f, 1.855600f, 0.000000f),
            new Vector4(1.574800f, -0.468124f, -0.000000f, 0.000000f),
            new Vector4(-0.787400f, 0.327724f, -0.927800f, 1.000000f)
        );

        public static readonly Matrix4x4 BT2020_YCbCrFromRGB = new Matrix4x4(
            new Vector4(0.262700f, -0.139630f, 0.500000f, 0.000000f),
            new Vector4(0.678000f, -0.360370f, -0.459786f, 0.000000f),
            new Vector4(0.059300f, 0.500000f, -0.040214f, 0.000000f),
            new Vector4(0.000000f, 0.500000f, 0.500000f, 1.000000f)
        );

        public static readonly Matrix4x4 BT2020_RGBFromYCbCr = new Matrix4x4(
            new Vector4(1.000000f, 1.000000f, 1.000000f, 0.000000f),
            new Vector4(-0.000000f, -0.164553f, 1.881400f, 0.000000f),
            new Vector4(1.474600f, -0.571353f, 0.000000f, 0.000000f),
            new Vector4(-0.737300f, 0.367953f, -0.940700f, 1.000000f)
        );
        public static Matrix4x4 GetRGBToYUVMatrix(Standard standard) {
            return standard switch {
                Standard.BT601 => BT601_YCbCrFromRGB,
                Standard.BT709 => BT709_YCbCrFromRGB,
                Standard.BT2020 => BT2020_YCbCrFromRGB,
                _ => BT709_YCbCrFromRGB,
            };
        }
        
        public static Matrix4x4 GetYUVToRGBMatrix(Standard standard) {
            return standard switch {
                Standard.BT601 => BT601_RGBFromYCbCr,
                Standard.BT709 => BT709_RGBFromYCbCr,
                Standard.BT2020 => BT2020_RGBFromYCbCr,
                _ => BT709_RGBFromYCbCr,
            };
        }

        private static void GetQuantizationLinearTransform(
                int bitsPerComponent, double min, double max,
                out double scale, out double offset) {

            if (bitsPerComponent < 8) {
                throw new System.ArgumentOutOfRangeException(nameof(bitsPerComponent));
            }

            int n = bitsPerComponent;

            // The following expressions are adapted from:
            //
            // https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf
            // Recommendation ITU-R BT.709-6 (06/2015): Parameter values for the HDTV standards for
            // production and international programme exchange
            // Section 3 -- Signal format, 3.4 -- Quantization of RGB, luminance and colour-difference signals

            double k = (double)(1 << (n - 8)) / ((1 << n) - 1);
            scale = (max - min) * k;
            offset = min * k;
        }

        private static void GetInvQuantizationLinearTransform(
                int bitsPerComponent, double min, double max,
                out double scale, out double offset) {

            if (bitsPerComponent < 8) {
                throw new System.ArgumentOutOfRangeException(nameof(bitsPerComponent));
            }

            int n = bitsPerComponent;

            // Ditto. See the GetQuantizationLinearTransform comment.
            double kInv = (double)((1 << n) - 1) / (1 << (n - 8));
            scale = kInv / (max - min);
            offset = -min / (max - min);
        }

        public static Matrix4x4 GetQuantizationMatrix(int bitsPerComponent) {

            // https://www.itu.int/dms_pubrec/itu-r/rec/bt/R-REC-BT.709-6-201506-I!!PDF-E.pdf
            // Recommendation ITU-R BT.709-6 (06/2015): Parameter values for the HDTV standards for
            // production and international programme exchange
            // Section 4 -- Digital representation, 4.6 -- Quantization levels.
            const double minY = 16;
            const double maxY = 235;
            const double minC = 16;
            const double maxC = 240;

            GetQuantizationLinearTransform(bitsPerComponent, minY, maxY,
                out double kY, out double bY);

            GetQuantizationLinearTransform(bitsPerComponent, minC, maxC,
                out double kC, out double bC);

            var m = Matrix4x4.zero;
            m[0, 0] = (float)kY;
            m[1, 1] = (float)kC;
            m[2, 2] = (float)kC;

            m[0, 3] = (float)bY;
            m[1, 3] = (float)bC;
            m[2, 3] = (float)bC;
            m[3, 3] = 1.0f;

            return m;
        }

        public static Matrix4x4 GetInvQuantizationMatrix(int bitsPerComponent) {

            // Ditto. See the GetQuantizationMatrix comment.
            const double minY = 16;
            const double maxY = 235;
            const double minC = 16;
            const double maxC = 240;

            GetInvQuantizationLinearTransform(bitsPerComponent, minY, maxY,
                out double kY, out double bY);

            GetInvQuantizationLinearTransform(bitsPerComponent, minC, maxC,
                out double kC, out double bC);

            var m = Matrix4x4.zero;
            m[0, 0] = (float)kY;
            m[1, 1] = (float)kC;
            m[2, 2] = (float)kC;

            m[0, 3] = (float)bY;
            m[1, 3] = (float)bC;
            m[2, 3] = (float)bC;
            m[3, 3] = 1.0f;

            return m;
        }

        public static Matrix4x4 GetRGBToYUVMatrix(Standard standard, int? bitsPerComponent) {

            var m = GetRGBToYUVMatrix(standard);
            if (bitsPerComponent == null) {
                return m;
            }

            var quantMatrix = GetQuantizationMatrix(bitsPerComponent.Value);
            return quantMatrix * m;
        }

        public static Matrix4x4 GetYUVToRGBMatrix(Standard standard, int? bitsPerComponent) {

            var m = GetYUVToRGBMatrix(standard);
            if (bitsPerComponent == null) {
                return m;
            }

            var unquantMatrix = GetInvQuantizationMatrix(bitsPerComponent.Value);
            return m * unquantMatrix;
        }

        public static TextureRGBMultipliedByMatrix Pack(Texture? texture = null,
                Standard standard = Standard.BT709, int? bitsPerComponent = null) {

            return new TextureRGBMultipliedByMatrix(texture, GetRGBToYUVMatrix(standard, bitsPerComponent));
        }

        public static TextureRGBMultipliedByMatrix Unpack(Texture? texture = null,
                Standard standard = Standard.BT709, int? bitsPerComponent = null) {

            return new TextureRGBMultipliedByMatrix(texture, GetYUVToRGBMatrix(standard, bitsPerComponent));
        }



    }
}
