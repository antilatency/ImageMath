using System;

using UnityEngine;
using UnityEngine.Experimental.Rendering;

using static ImageMath.Static;
#nullable enable
namespace ImageMath {

    public class LUT3D : LUT3DBase {
        public Texture3D Texture { get; protected set; } = null!; // Initialized in CreateTexture
        public LUT3D(int size) : base(size) {
        }

        protected override void CreateTexture(int size) {
            Texture = Static.CreateTexture3D(new Vector3Int(size, size, size), GraphicsFormat.R32G32B32A32_SFloat, false);
        }
        public override void SetData(Vector4[] cells, bool apply = true) {
            Texture.SetPixelData(cells);
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

        public static LUT3D? CreateFromCubeFileContent(string content, bool apply = true) {
            return ParseLUT3D(content, (size) => new LUT3D(size));
        }
    }

}
