using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
#nullable enable
namespace ImageMath {
    public class FlatLUT3D : FlatLUT3DBase {
        public Texture2D Texture { get; protected set; } = null!; // Initialized in CreateTexture
        public FlatLUT3D(int size) : base(size) {
        }

        protected override void CreateTexture(int size) {
            var dimensions = CalculateDimensions(size);
            Texture = Static.CreateTexture2D(dimensions.x, dimensions.y, GraphicsFormat.R32G32B32A32_SFloat, false);
        }
        public override void SetData(Vector4[] cells, bool apply = true) {
            Texture.SetPixelData(cells, 0);
            if (apply) {
                Texture.Apply();
            }
        }
        public override Vector4[] GetData() {
            var data = Texture.GetPixelData<Vector4>(0);
            return data.ToArray();
        }
        public override void DestroyTexture() {
            if (Texture) {
                GameObject.DestroyImmediate(Texture);
            }
        }

        public static FlatLUT3D? CreateFromCubeFileContent(string content, bool apply = true) {
            return ParseLUT3D(content,
                (size) => new FlatLUT3D(size));
        }

        public FlatLUT3DRenderable CreateCompatibleRenderable() {
            return new FlatLUT3DRenderable(Size);
        }

    }

}
