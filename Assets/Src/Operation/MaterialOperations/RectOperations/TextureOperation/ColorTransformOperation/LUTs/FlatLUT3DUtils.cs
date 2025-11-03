using System;
using System.IO;

using UnityEngine;
using UnityEngine.Experimental.Rendering;
#nullable enable
namespace ImageMath {
    public static class FlatLUT3D {
        public static Vector2Int CalculateDimensions(int size) {
            const int MaxTextureSize = 16384;
            int numPixels = size * size * size;
            int width;
            int height;

            if (size * size <= MaxTextureSize) {
                width = size;
                height = size * size;
            }
            else {//130 is max size
                float sideF = Mathf.Sqrt(numPixels);
                width = Mathf.CeilToInt(sideF);
                for (int w = width; w >= 1; w--) {
                    if (numPixels % w == 0) {
                        width = w;
                        break;
                    }
                }
                height = numPixels / width;
                if (height > MaxTextureSize) {
                    throw new Exception($"LUT size {size} too large.");
                }
            }
            return new Vector2Int(width, height);
        }

        public static int CalculateSizeFromTextureSize(Vector2Int textureSize) {
            var totalPixels = textureSize.x * textureSize.y;
            int size = Mathf.RoundToInt(Mathf.Pow(totalPixels, 1.0f / 3.0f));
            if (size * size * size != totalPixels) {
                throw new Exception("Texture dimensions do not correspond to a valid 3D LUT size.");
            }
            return size;
        }

        public static int CalculateSizeFromTexture(Texture texture) => CalculateSizeFromTextureSize(new Vector2Int(texture.width, texture.height));
        

        public static Texture3D ToLUT3D(this Texture texture) {
            if (texture == null) {
                throw new ArgumentNullException(nameof(texture));
            }
            if (!(texture is Texture2D texture2D) && !(texture is RenderTexture renderTexture)) {
                throw new ArgumentException("Texture must be a Texture2D or RenderTexture.", nameof(texture));
            }
            int size = CalculateSizeFromTexture(texture);
            var data = texture.GetPixelData<Vector4>(0);
            var result = new Texture3D(size, size, size, GraphicsFormat.R32G32B32A32_SFloat, TextureCreationFlags.None);
            result.SetPixelData(data, 0);
            return result;
        }

        public static Vector4[]? ParseLUT3D(string content, out int size, out Vector3 domainMin, out Vector3 domainMax, out string? title) {
            var reader = new StringReader(content);
            //parse header
            bool firstNonHeaderLineFound = false;
            string? line = null;
            title = null;
            size = 0;
            domainMin = Vector3.zero;
            domainMax = Vector3.one;
            while (!firstNonHeaderLineFound) {
                line = reader.ReadLine();
                if (line == null) {
                    return null;
                }

                int indexOfComment = line.IndexOf("#");
                if (indexOfComment == 0) {
                    continue;
                }
                if (indexOfComment > 0) {
                    line = line.Substring(0, indexOfComment);
                }

                //parse name fromstart to first space like separator
                var parts = SplitByWhitespace(line);
                string name = parts[0];
                switch (name) {
                    case "TITLE":
                        title = line.Substring(6).Trim();
                        break;
                    case "DOMAIN_MIN":
                        domainMin = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                        break;
                    case "DOMAIN_MAX":
                        domainMax = new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3]));
                        break;
                    case "LUT_3D_SIZE":
                        size = int.Parse(parts[1]);
                        break;
                    case "LUT_3D_INPUT_RANGE":
                        var min = float.Parse(parts[1]);
                        var max = float.Parse(parts[2]);
                        domainMin = new Vector3(min, min, min);
                        domainMax = new Vector3(max, max, max);
                        break;
                    default:
                        firstNonHeaderLineFound = true;
                        break;
                }
            }

            if (size == 0) {
                return null;
            }

            Vector4[] data = new Vector4[size * size * size];
            for (int i = 0; i < data.Length; i++) {

                if (line == null) {
                    return null;
                }
                var parts = SplitByWhitespace(line);
                data[i] = new Vector4(float.Parse(parts[0]), float.Parse(parts[1]), float.Parse(parts[2]), 1f);
                line = reader.ReadLine();
            }



            return data;
        }


        private static string[] SplitByWhitespace(string line) {
            return line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ToCubeFileContent(this Texture texture, Vector3 domainMin, Vector3 domainMax, string? title = null) {
            var result = new System.Text.StringBuilder();
            var size = CalculateSizeFromTexture(texture);
            result.AppendLine($"LUT_3D_SIZE {size}");
            result.AppendLine($"DOMAIN_MIN {domainMin.x} {domainMin.y} {domainMin.z}");
            result.AppendLine($"DOMAIN_MAX {domainMax.x} {domainMax.y} {domainMax.z}");
            result.AppendLine($"TITLE {title}");

            var cells = texture.GetPixelData<Vector4>(0);
            for (int i = 0; i < cells.Length; i++) {
                var cell = cells[i];
                result.AppendLine($"{cell.x} {cell.y} {cell.z}");
            }

            return result.ToString();
        }


    }

}
