using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Assertions;
# nullable enable
namespace ImageMath {
    [System.Serializable]
    public class Spectrum : System.IEquatable<Spectrum> {

        public static string FileExtension = "spectrum";



        public float StartWavelength = 380;
        [Newtonsoft.Json.JsonIgnore]
        public float WavelengthEnd => StartWavelength + WavelengthStep * (Values.Length - 1);
        public float Width => WavelengthStep * (Values.Length - 1);
        public float WavelengthStep = 1;
        public float[] Values = new float[401];
        public Spectrum() { }

        public Spectrum(Spectrum other) {
            StartWavelength = other.StartWavelength;
            WavelengthStep = other.WavelengthStep;
            Values = new float[other.Values.Length];
            for (int i = 0; i < other.Values.Length; i++) {
                Values[i] = other.Values[i];
            }
        }

        public Spectrum(IList<float> values, float startWavelength = 380, float wavelengthStep = 1) {
            StartWavelength = startWavelength;
            WavelengthStep = wavelengthStep;
            Values = new float[values.Count];
            for (int i = 0; i < values.Count; i++) {
                Values[i] = values[i];
            }
        }
        public Spectrum(float startWavelength, int count = 401, float wavelengthStep = 1) {
            StartWavelength = startWavelength;
            WavelengthStep = wavelengthStep;
            Values = new float[count];
        }

        public float Sample(float wavelength, float defaultValue = 0) {
            int index = (int)((wavelength - StartWavelength) / WavelengthStep);
            int nextIndex = index + 1;
            float t = (wavelength - (StartWavelength + index * WavelengthStep)) / WavelengthStep;

            float value, nextValue;
            if (index < 0 || index >= Values.Length) {
                value = defaultValue;
            }
            else {
                value = Values[index];
            }
            if (nextIndex < 0 || nextIndex >= Values.Length) {
                nextValue = defaultValue;
            }
            else {
                nextValue = Values[nextIndex];
            }
            return value * (1 - t) + nextValue * t;
        }

        public Spectrum Resample(float newStartWavelength, float newWavelengthStep, int newCount, float defaultValue = 0) {
            Spectrum resampled = new Spectrum();
            resampled.StartWavelength = newStartWavelength;
            resampled.WavelengthStep = newWavelengthStep;
            resampled.Values = new float[newCount];
            for (int i = 0; i < newCount; i++) {
                float wavelength = newStartWavelength + i * newWavelengthStep;
                resampled.Values[i] = Sample(wavelength, defaultValue);
            }
            return resampled;
        }
        public Spectrum ResampleToDomainOf(Spectrum other, float defaultValue = 0) {
            return Resample(other.StartWavelength, other.WavelengthStep, other.Values.Length, defaultValue);
        }

        private static Spectrum SimpleAddition(Spectrum a, Spectrum b) {
            AssertSameDomain(a, b);
            Spectrum result = new Spectrum(a);
            for (int i = 0; i < a.Values.Length; i++) {
                result.Values[i] += b.Values[i];
            }
            return result;
        }

        public static bool SameDomain(Spectrum a, Spectrum b) {
            return a.StartWavelength == b.StartWavelength && a.WavelengthStep == b.WavelengthStep && a.Values.Length == b.Values.Length;
        }

        public static void AssertSameDomain(Spectrum a, Spectrum b) {
            Assert.AreEqual(a.StartWavelength, b.StartWavelength);
            Assert.AreEqual(a.WavelengthStep, b.WavelengthStep);
            Assert.AreEqual(a.Values.Length, b.Values.Length);
        }

        public static Spectrum operator +(Spectrum a, Spectrum b) {
            if (SameDomain(a, b)) {
                return SimpleAddition(a, b);
            }
            else {
                var bResampled = b.Resample(a.StartWavelength, a.WavelengthStep, a.Values.Length);
                return SimpleAddition(a, bResampled);
            }
        }

        public static Spectrum SimpleMultiplication(Spectrum a, Spectrum b) {
            AssertSameDomain(a, b);
            Spectrum result = new Spectrum(a);
            for (int i = 0; i < a.Values.Length; i++) {
                result.Values[i] *= b.Values[i];
            }
            return result;
        }

        public static Spectrum operator *(Spectrum a, Spectrum b) {
            if (SameDomain(a, b)) {
                return SimpleMultiplication(a, b);
            }
            else {
                var bResampled = b.Resample(a.StartWavelength, a.WavelengthStep, a.Values.Length);
                return SimpleMultiplication(a, bResampled);
            }
        }

        public static Spectrum operator *(Spectrum a, float scalar) {
            Spectrum result = new Spectrum(a);
            for (int i = 0; i < a.Values.Length; i++) {
                result.Values[i] *= scalar;
            }
            return result;
        }

        public static Spectrum operator /(Spectrum a, float scalar) {
            Spectrum result = new Spectrum(a);
            for (int i = 0; i < a.Values.Length; i++) {
                result.Values[i] /= scalar;
            }
            return result;
        }

        public Spectrum Normalized() {
            var max = Values.Max();
            if (max == 0) {
                return new Spectrum(this);
            }
            else {
                return this / max;
            }
        }

        //compare two spectrums for equality
        public override bool Equals(object? obj) {
            if (obj is Spectrum other) {
                if (!SameDomain(this, other)) {
                    return false;
                }
                for (int i = 0; i < Values.Length; i++) {
                    if (Values[i] != other.Values[i]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
        public override int GetHashCode() {
            int hash = 17;
            hash = hash * 31 + StartWavelength.GetHashCode();
            hash = hash * 31 + WavelengthStep.GetHashCode();
            foreach (var value in Values) {
                hash = hash * 31 + value.GetHashCode();
            }
            return hash;
        }

        public bool Equals(Spectrum other) {
            return this.Equals((object?)other);
        }

        public static bool operator ==(Spectrum? a, Spectrum? b) {
            if (ReferenceEquals(a, b)) {
                return true;
            }
            if (a is null || b is null) {
                return false;
            }
            return a.Equals(b);
        }
        public static bool operator !=(Spectrum? a, Spectrum? b) {
            return !(a == b);
        }

        public string ToJson() {
            //. as float separator
            var settings = new Newtonsoft.Json.JsonSerializerSettings {
                Culture = System.Globalization.CultureInfo.InvariantCulture
            };
            return Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented, settings);
        }

        public static Color WavelengthToVisualizationColor(float wavelength) {
            var gradient = new (float wavelength, Color color)[] {
                (380, new Color(1f, 0f, 1f, 0f)), // Violet invisible
                (440, new Color(0.29f, 0f, 1f, 1f)), // Blue
                (490, new Color(0f, 1f, 1f, 1f)), // Cyan
                (510, new Color(0f, 1f, 0f, 1f)), // Green
                (570, new Color(1f, 1f, 0f, 1f)), // Yellow
                (590, new Color(1f, 0.65f, 0f, 1f)), // Orange
                (620, new Color(1f, 0f, 0f, 1f)), // Red
                (750, new Color(1f, 0f, 0f, 0f)) // Deep Red invisible                
            };
            if (wavelength < gradient[0].wavelength || wavelength > gradient[gradient.Length - 1].wavelength) {
                return new Color(0f, 0f, 0f, 0f);
            }
            for (int i = 1; i < gradient.Length; i++) {
                if (wavelength <= gradient[i].wavelength) {
                    var left = gradient[i - 1];
                    var right = gradient[i];
                    float t = (wavelength - left.wavelength) / (right.wavelength - left.wavelength);
                    var color = Color.Lerp(left.color, right.color, t);
                    color *= color.a;
                    color.a = 1.0f;
                    return color;
                }
            }
            return new Color(0f, 0f, 0f, 0f);
        }

        public Color[] GetVisualizationColors() {
            Color[] colors = new Color[Values.Length];
            for (int i = 0; i < Values.Length; i++) {
                float wavelength = StartWavelength + i * WavelengthStep;
                colors[i] = WavelengthToVisualizationColor(wavelength); 
            }
            return colors;
        }

        public float SpectrumSimilarityIndex(Spectrum reference) {
            /*
https://oscars.org/sites/oscars/files/ssi_overview_2020-09-16.pdf

1) Specify test and reference source SPDs (at intervals not exceeding 5 nm).
2) Interpolate spectra to 1-nm increments from 375 nm to 675 nm (padding with zeroes if the test
luminaire is not specified fully across that range).
3) Integrate spectra in 10-nm intervals from 380 to 670 nm.
4) Normalize to unity total power of test and reference sources by dividing each 10-nm sample by
sum of all 10-nm samples for each source.
5) Calculate relative difference vector = (normalized test source vector – normalized reference
source vector) / (normalized reference source vector + 1/30).
6) Calculate weighted relative difference vector = relative difference vector * spectral weighting
vector
{4/15, 22/45, 32/45, 8/9, 44/45, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 11/15, 3/15}.
7) Add zero to each end of weighted relative difference vector to have 32 values.
8) Convolve with {0.22, 0.56, 0.22} to create 30-element smoothed weighted relative difference
vector.
9) Calculate vector magnitude = square root of sum of squares of elements of smoothed weighted
relative difference vector.
10) SSI value = round (100 – 32 * vector magnitude). 

            */
            const int startWavelength = 375;
            const int endWavelength = 675;
            
            var a = this.Resample(startWavelength, 1, endWavelength - startWavelength);
            var b = reference.Resample(startWavelength, 1, endWavelength - startWavelength);
            var weights = new double[] { 12.0 / 45.0, 22.0 / 45.0, 32.0 / 45.0, 40.0 / 45.0, 44.0 / 45.0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 11.0 / 15.0, 3.0 / 15.0 };
            var aBins = new double[weights.Length];
            var bBins = new double[weights.Length];
            double aSum = 0;
            double bSum = 0;
            for (int i = 0; i < weights.Length; i++) {
                for (int j = 0; j < 10; j++) {
                    aBins[i] += a.Values[i * 10 + j];
                    bBins[i] += b.Values[i * 10 + j];
                }
                aSum += aBins[i];
                bSum += bBins[i];
            }
            for (int i = 0; i < weights.Length; i++) {
                aBins[i] /= aSum;
                bBins[i] /= bSum;
            }
            double[] difference = new double[weights.Length];
            for (int i = 0; i < weights.Length; i++) {
                difference[i] = aBins[i] - bBins[i];
            }
            double bMean = bBins.Average();
            double[] relativeDifference = new double[weights.Length];
            for (int i = 0; i < weights.Length; i++) {
                relativeDifference[i] = difference[i] / (bBins[i] + bMean);
            }
            double[] weightedRelativeDifference = new double[weights.Length];
            for (int i = 0; i < weights.Length; i++) {
                weightedRelativeDifference[i] = relativeDifference[i] * weights[i];
            }

            double[] convolveKernel = new double[] { 0.22, 0.56, 0.22 };
            double[] smoothed = new double[weights.Length];
            for (int i = 0; i < weights.Length; i++) {
                for (int j = 0; j < convolveKernel.Length; j++) {
                    int index = i + j - 1;
                    if (index >= 0 && index < weights.Length) {
                        smoothed[i] += weightedRelativeDifference[index] * convolveKernel[j];
                    }
                }
            }

            double sumOfSquares = 0;
            for (int i = 0; i < weights.Length; i++) {
                sumOfSquares += smoothed[i] * smoothed[i];
            }
            double vectorMagnitude = System.Math.Sqrt(sumOfSquares);
            double ssi = 100 - 32 * vectorMagnitude;

            return (float)ssi;
        }

    }

}
