using System;
using System.IO;
using UnityEngine;
#nullable enable
namespace ImageMath {
    [Serializable]
    public abstract class LUT3DBase : IDisposable {
        public abstract Texture Texture { get; }
        public string? Title { get; set; }
        public Vector3 DomainMin { get; set; } = Vector3.zero;
        public Vector3 DomainMax { get; set; } = Vector3.one;
        //public T Texture { get; protected set; }
        public int Size { get; private set; } = 0;

        //Texture not null

        public LUT3DBase(int size) {
            Size = size;
            CreateTexture(size);
        }

        protected abstract void CreateTexture(int size);
        public abstract void SetData(Vector4[] cells, bool apply = true);
        public abstract Vector4[] GetData();
        public abstract void DestroyTexture();

        public void Dispose() {
            DestroyTexture();
            /*if (Texture) {
                GameObject.DestroyImmediate(Texture);
            }*/
        }

        /*public void SetData(Vector4[] cells, bool apply = true) {
            if (Texture == null) {
                throw new Exception("LUT3DFlat: Texture is null, cannot set pixel data.");
            }

            Texture.SetRawTextureData(cells,apply);
        }*/




        public static R? ParseLUT3D<R>(string content, Func<int, R> constructor) where R : LUT3DBase {
            var reader = new StringReader(content);
            //parse header
            bool firstNonHeaderLineFound = false;
            string? line = null;
            string? title = null;
            int size = 0;
            Vector3 domainMin = Vector3.zero;
            Vector3 domainMax = Vector3.one;
            while (!firstNonHeaderLineFound) {
                line = reader.ReadLine();
                if (line == null) {
                    return null;
                }
                if (line.StartsWith("#")) {
                    continue;
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

            var result = constructor(size);
            result.Title = title;
            result.DomainMin = domainMin;
            result.DomainMax = domainMax;
            result.SetData(data, true);

            return result;
        }


        private static string[] SplitByWhitespace(string line) {
            return line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string ToCubeFileContent(LUT3DBase lut) {
            var result = new System.Text.StringBuilder();

            result.AppendLine($"LUT_3D_SIZE {lut.Size}");
            result.AppendLine($"DOMAIN_MIN {lut.DomainMin.x} {lut.DomainMin.y} {lut.DomainMin.z}");
            result.AppendLine($"DOMAIN_MAX {lut.DomainMax.x} {lut.DomainMax.y} {lut.DomainMax.z}");
            result.AppendLine($"TITLE {lut.Title}");

            var cells = lut.GetData();
            for (int i = 0; i < lut.Size * lut.Size * lut.Size; i++) {
                var cell = cells[i];
                result.AppendLine($"{cell.x} {cell.y} {cell.z}");
            }

            return result.ToString();
        }

    }

}
