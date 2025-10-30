using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
#nullable enable
namespace ImageMath {
    [Serializable]
    public class FlatLUT3D : FlatLUT3DBase {
        public override Texture Texture => Texture2D;
        public Texture2D Texture2D { get; protected set; } = null!; // Initialized in CreateTexture
        public FlatLUT3D(int size) : base(size) {
        }

        protected override void CreateTexture(int size) {
            var dimensions = CalculateDimensions(size);
            Texture2D = Static.CreateTexture2D(dimensions.x, dimensions.y, GraphicsFormat.R32G32B32A32_SFloat, false);
        }
        public override void SetData(Vector4[] cells, bool apply = true) {
            Texture2D.SetPixelData(cells, 0);
            if (apply) {
                Texture2D.Apply();
            }
        }
        public override Vector4[] GetData() {
            var data = Texture2D.GetPixelData<Vector4>(0);
            return data.ToArray();
        }
        public override void DestroyTexture() {
            if (Texture2D) {
                GameObject.DestroyImmediate(Texture2D);
            }
        }

        public static FlatLUT3D? CreateFromCubeFileContent(string content, bool apply = true) {
            return ParseLUT3D(content,
                (size) => new FlatLUT3D(size));
        }

        public FlatLUT3DRenderable CreateCompatibleRenderable() {
            return new FlatLUT3DRenderable(Size);
        }

        public void InitializeIdentity(bool apply = true) {
            var cells = new Vector4[Size * Size * Size];
            float invSizeMinusOne = 1.0f / (Size - 1);
            for (int r = 0; r < Size; r++) {
                for (int g = 0; g < Size; g++) {
                    for (int b = 0; b < Size; b++) {
                        int index = r + g * Size + b * Size * Size;
                        cells[index] = new Vector4(
                            r * invSizeMinusOne,
                            g * invSizeMinusOne,
                            b * invSizeMinusOne,
                            1.0f);
                    }
                }
            }
            SetData(cells, apply);
        }

        public static FlatLUT3D CreateIdentity(int size, bool apply = true) {
            var flatLUT = new FlatLUT3D(size);
            flatLUT.InitializeIdentity(apply);
            return flatLUT;
        }

    }

}
