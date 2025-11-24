using System;
using System.Linq;

using ImageMath.ScriptableObjects;

using UnityEditor;

using UnityEngine;

# nullable enable

namespace ImageMath {
    [CustomEditor(typeof(ScriptableSpectrum))]
    public class ScriptableSpectrumEditor : Editor {


        public override bool HasPreviewGUI() => true;

        public override void OnPreviewGUI(Rect r, GUIStyle background) {
            ScriptableSpectrum scriptable = (ScriptableSpectrum)target;
            var spectrum = scriptable.Value;
            EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, r.height), Color.black);
                //flip y axis using matrix
            var oldMatrix = Handles.matrix;
            Handles.matrix = Matrix4x4.TRS(new Vector3(0, r.y + r.height, 0), Quaternion.identity, new Vector3(1, -1, 1)) * oldMatrix;  


            Handles.color = Color.white;


            var widthPerNanometer = r.width / (spectrum.WavelengthEnd - spectrum.StartWavelength);
            float maxValue = -float.MaxValue;
            float peakWavelength = spectrum.StartWavelength;
            float averageWavelength = 0;
            float averageWavelengthDenominator = 0;
            for (int i = 0; i < spectrum.Values.Length; i++) {
                var wavelength = spectrum.StartWavelength + i * spectrum.WavelengthStep;
                var value = spectrum.Values[i];
                if (value > maxValue) {
                    maxValue = value;
                    peakWavelength = wavelength;
                }
                averageWavelength += wavelength * value;
                averageWavelengthDenominator += value;
            }
            averageWavelength /= averageWavelengthDenominator;

            (spectrum / maxValue).DrawSpectrumUsingHandles(r.height, widthPerNanometer);
            Handles.Label(
                new Vector3((averageWavelength - spectrum.StartWavelength) * widthPerNanometer, r.height * 0.5f, 0)
                , $"Peak: {peakWavelength} nm\nAvg: {averageWavelength:F1} nm");

            Handles.matrix = oldMatrix;
        }

        public override Texture2D RenderStaticPreview(string assetPath, UnityEngine.Object[] subAssets, int width, int height){
            var scriptable = (ScriptableSpectrum)target;
            var spectrum = scriptable.Value.Normalized();
            var step = spectrum.Width / width;
            var resampled = spectrum.Resample(spectrum.StartWavelength, step, width);
            var result = new Texture2D(width, height
                , UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB
                , UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
            
            UInt32[] pixels = new UInt32[width * height];
            UInt32[] colors = resampled.GetVisualizationColors().Select(c => {
                Color32 cc = c;
                return (UInt32)((255 << 24) | (cc.r) | (cc.g << 8) | (cc.b << 16));
            }).ToArray();

            for (int x = 0; x < width; x++) {

                float value = resampled.Values[x];
                int yMax = Mathf.Clamp((int)(value * height), 0, height);
                for (int y = 0; y < yMax; y++) {
                    pixels[y * width + x] = colors[x];
                }
            }
            result.SetPixelData<UInt32>(pixels, 0);
            result.Apply();
            return result;
        }
    }



}
