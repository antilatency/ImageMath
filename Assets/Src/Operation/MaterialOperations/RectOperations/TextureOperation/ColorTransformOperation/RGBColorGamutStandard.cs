#nullable enable

using System;

using UnityEngine;

namespace ImageMath {

    // NOTE: The order and encoding is not stable and WILL change in the near future.
    // Do not rely on the numeric values of these enums for serialization or other purposes.
    public enum RGBColorGamutStandard {

        // CIE 1931:
        CIE1931_XYZ_E,
        CIE1931_XYZ_D65,

        // ACES:
        ACES_AP0,
        ACES_AP1,

        // sRGB:
        sRGB,

        // ITU-R:
        Rec709,
        Rec2020,

        // ARRI:
        ArriWideGamut3,
        ArriWideGamut4,

        // Blackmagic Design:
        BlackMagicDesignWideGamut,
    }

    public class RGBColorGamutStandards {

        public static RGBColorGamut GetGamut(RGBColorGamutStandard standard) {
            switch (standard) {
                case RGBColorGamutStandard.CIE1931_XYZ_E:
                    return new RGBColorGamut(
                        r: new Vector2(1.0f, 0.0f),
                        g: new Vector2(0.0f, 1.0f),
                        b: new Vector2(0.0f, 0.0f),
                        whitePoint: new Vector2(1.0f / 3.0f, 1.0f / 3.0f)
                    );

                case RGBColorGamutStandard.ACES_AP0:
                    return new RGBColorGamut(
                        r: new Vector2(0.7347f, 0.2653f),
                        g: new Vector2(0f, 1f),
                        b: new Vector2(0.0001f, -0.0770f),
                        whitePoint: new Vector2(0.32168f, 0.33767f)
                    );

                case RGBColorGamutStandard.ACES_AP1:
                    return new RGBColorGamut(
                        r: new Vector2(0.713f, 0.293f),
                        g: new Vector2(0.165f, 0.830f),
                        b: new Vector2(0.128f, 0.044f),
                        whitePoint: new Vector2(0.32168f, 0.33767f)
                    );

                case RGBColorGamutStandard.CIE1931_XYZ_D65:
                    return new RGBColorGamut(
                        r: new Vector2(1.0f, 0.0f),
                        g: new Vector2(0.0f, 1.0f),
                        b: new Vector2(0.0f, 0.0f),
                        whitePoint: new Vector2(0.3127f, 0.3290f)
                    );

                case RGBColorGamutStandard.sRGB:
                    return new RGBColorGamut(
                        r: new Vector2(0.6400f, 0.3300f),
                        g: new Vector2(0.3000f, 0.6000f),
                        b: new Vector2(0.1500f, 0.0600f),
                        whitePoint: new Vector2(0.3127f, 0.3290f)
                    );

                case RGBColorGamutStandard.Rec709:
                    return new RGBColorGamut(
                        r: new Vector2(0.640f, 0.330f),
                        g: new Vector2(0.300f, 0.600f),
                        b: new Vector2(0.150f, 0.060f),
                        whitePoint: new Vector2(0.3127f, 0.3290f)
                    );

                case RGBColorGamutStandard.Rec2020:
                    return new RGBColorGamut(
                        r: new Vector2(0.708f, 0.292f),
                        g: new Vector2(0.170f, 0.797f),
                        b: new Vector2(0.131f, 0.046f),
                        whitePoint: new Vector2(0.3127f, 0.3290f)
                    );

                case RGBColorGamutStandard.ArriWideGamut3:
                    return new RGBColorGamut(
                        r: new Vector2(0.6840f, 0.3130f),
                        g: new Vector2(0.2210f, 0.8480f),
                        b: new Vector2(0.0861f, -0.1020f),
                        whitePoint: new Vector2(0.3127f, 0.3290f)
                    );

                case RGBColorGamutStandard.ArriWideGamut4:
                    return new RGBColorGamut(
                        r: new Vector2(0.7347f, 0.2653f),
                        g: new Vector2(0.1424f, 0.8576f),
                        b: new Vector2(0.0991f, -0.0308f),
                        whitePoint: new Vector2(0.3127f, 0.3290f)
                    );

                case RGBColorGamutStandard.BlackMagicDesignWideGamut:
                    return new RGBColorGamut(
                        r: new Vector2(0.7177215f, 0.3171181f),
                        g: new Vector2(0.2280410f, 0.8615690f),
                        b: new Vector2(0.1005841f, -0.0820452f),
                        whitePoint: new Vector2(0.3127170f, 0.3290312f)
                    );

                default:
                    throw new ArgumentException($"Unknown RGB color gamut standard: {standard}");
            }
        }
    }
}
