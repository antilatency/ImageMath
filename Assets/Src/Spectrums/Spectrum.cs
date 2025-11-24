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
        public float Width => WavelengthStep * Values.Length;
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

    }

}
