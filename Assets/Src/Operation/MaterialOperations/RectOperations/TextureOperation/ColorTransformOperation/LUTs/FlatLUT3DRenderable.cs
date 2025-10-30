using System;

using UnityEngine;
#nullable enable
namespace ImageMath {
    [Serializable]
    public class FlatLUT3DRenderable : FlatLUT3DBase {
        public override Texture Texture => _renderTexture;

        private RenderTexture _renderTexture = null!; // Initialized in CreateTexture
        public RenderTexture RenderTexture { get => _renderTexture; protected set => _renderTexture = value; }
        public FlatLUT3DRenderable(int size) : base(size) {
        }

        protected override void CreateTexture(int size) {
            var dimensions = CalculateDimensions(size);
            RenderTexture = Static.CreateRenderTexture(dimensions.x, dimensions.y);
        }
        public override void SetData(Vector4[] cells, bool _ = true) {
            RenderTexture.SetPixelData(cells, 0);
        }
        public override Vector4[] GetData() {
            var data = RenderTexture.GetPixelData<Vector4>(0);
            return data;
        }
        public override void DestroyTexture() {
            if (RenderTexture) {
                UnityEngine.Object.DestroyImmediate(RenderTexture);
            }
        }
    }

}
