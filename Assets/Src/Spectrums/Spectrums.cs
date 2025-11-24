using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

# nullable enable
namespace ImageMath {

    public static class Spectrums {
        public static Spectrum DotProduct(IList<Spectrum> spectrums, IList<float> weights) {
            Assert.AreEqual(spectrums.Count, weights.Count);
            var resampledSpectrums = new Spectrum[spectrums.Count];
            resampledSpectrums[0] = new Spectrum(spectrums[0]);
            for (int i = 1; i < spectrums.Count; i++) {
                if (Spectrum.SameDomain(spectrums[0], spectrums[i])) {
                    resampledSpectrums[i] = spectrums[i];
                }
                else {
                    resampledSpectrums[i] = spectrums[i].ResampleToDomainOf(spectrums[0]);
                }
            }
            Spectrum result = new Spectrum(spectrums[0].StartWavelength, spectrums[0].Values.Length, spectrums[0].WavelengthStep);
            for (int i = 0; i < result.Values.Length; i++) {
                float value = 0;
                for (int j = 0; j < spectrums.Count; j++) {
                    value += resampledSpectrums[j].Values[i] * weights[j];
                }
                result.Values[i] = value;
            }
            return result;
        }

        public static Spectrum CreateBlackbodySpectrum(float temperatureKelvin, float startWavelength = 380, int count = 401, float wavelengthStep = 1) {
            if (temperatureKelvin <= 0)
                return new Spectrum(startWavelength, count, wavelengthStep);
            if (temperatureKelvin > 1e+07)
                temperatureKelvin = 1e+07f;
            Spectrum spectrum = new Spectrum(startWavelength, count, wavelengthStep);
            for (int i = 0; i < count; i++) {
                double wavelengthMeters = (startWavelength + i * wavelengthStep) * 1e-9f;
                // Planck's law
                double c1 = 3.741771e-16; // 2hc^2
                double c2 = 1.4387769e-2; // hc/k
                double exponent = c2 / (wavelengthMeters * temperatureKelvin);
                float intensity = (float)((c1 / (System.Math.Pow(wavelengthMeters, 5))) / (System.Math.Exp(exponent) - 1.0));
                spectrum.Values[i] = intensity;
            }
            return spectrum;
        }
    }

#if UNITY_EDITOR
    public static class SpectrumsDrawing {

        public static void DrawSpectrumUsingHandles(this Spectrum spectrum, float Height = 1.0f, float WidthPerNanometer = 0.01f) {
            Vector3[] points = new Vector3[spectrum.Values.Length];
            var iScale = spectrum.WavelengthStep * WidthPerNanometer;
            for (int i = 0; i < spectrum.Values.Length; i++) {
                float x = i * iScale;
                float y = spectrum.Values[i] * Height;
                points[i] = new Vector3(x, y, 0);
            }
            UnityEditor.Handles.DrawPolyLine(points);

        }
    }

#endif

}
