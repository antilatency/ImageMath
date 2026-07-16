#nullable enable

using UnityEngine;

namespace ImageMath {

    public static class RGBColorGamutOp {

        // The verb "Pack" in this file's function names means to convert from CIE1931_XYZ_E to
        // a specific RGB color gamut. The verb "Unpack" means the inverse operation. While any
        // triangular gamut encompassing the CIE 1931 horseshoe entirely could work as the
        // baseline, CIE1931_XYZ_E is the most natural choice mathematically.

        public static TextureMultipliedByMatrix PackRGBColorGamut(Texture? texture, RGBColorGamut colorGamut) {
            var matrix = colorGamut.CalcTransformMatrix(RGBColorGamut.TransformDirection.RGBFromXYZ);
            return new TextureMultipliedByMatrix(texture, matrix);
        }

        public static TextureMultipliedByMatrix PackRGBColorGamut(Texture? texture, RGBColorGamutStandard standard) {
            var gamut = RGBColorGamutStandards.GetGamut(standard);
            return PackRGBColorGamut(texture, gamut);
        }

        public static TextureMultipliedByMatrix UnpackRGBColorGamut(Texture? texture, RGBColorGamut colorGamut) {
            var matrix = colorGamut.CalcTransformMatrix(RGBColorGamut.TransformDirection.XYZFromRGB);
            return new TextureMultipliedByMatrix(texture, matrix);
        }

        public static TextureMultipliedByMatrix UnpackRGBColorGamut(Texture? texture, RGBColorGamutStandard standard) {
            var gamut = RGBColorGamutStandards.GetGamut(standard);
            return UnpackRGBColorGamut(texture, gamut);
        }

        public static TextureMultipliedByMatrix ConvertRGBColorGamut(Texture? texture, RGBColorGamut sourceGamut, RGBColorGamut targetGamut) {
            var sourceMatrix = sourceGamut.CalcTransformMatrix(RGBColorGamut.TransformDirection.XYZFromRGB);
            var targetMatrix = targetGamut.CalcTransformMatrix(RGBColorGamut.TransformDirection.RGBFromXYZ);
            var matrix = targetMatrix * sourceMatrix;
            return new TextureMultipliedByMatrix(texture, matrix);
        }

        public static TextureMultipliedByMatrix ConvertRGBColorGamut(Texture? texture, RGBColorGamutStandard sourceStandard, RGBColorGamutStandard targetStandard) {
            var sourceGamut = RGBColorGamutStandards.GetGamut(sourceStandard);
            var targetGamut = RGBColorGamutStandards.GetGamut(targetStandard);
            return ConvertRGBColorGamut(texture, sourceGamut, targetGamut);
        }
    }
}
