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

        public static TextureRGBMultipliedByMatrix Pack(Texture? texture = null, Standard standard = Standard.BT709) {
            return new TextureRGBMultipliedByMatrix(texture, GetRGBToYUVMatrix(standard));
        }

        public static TextureRGBMultipliedByMatrix Unpack(Texture? texture = null, Standard standard = Standard.BT709) {
            return new TextureRGBMultipliedByMatrix(texture, GetYUVToRGBMatrix(standard));
        }



    }
}
