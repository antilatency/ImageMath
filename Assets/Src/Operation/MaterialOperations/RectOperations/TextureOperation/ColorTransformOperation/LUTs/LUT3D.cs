using System;

using UnityEngine;
using UnityEngine.Experimental.Rendering;

using static ImageMath.Static;
#nullable enable
namespace ImageMath {

    public class LUT3D : LUT3DBase {
        public override Texture Texture => Texture3D;
        public Texture3D Texture3D { get; protected set; } = null!; // Initialized in CreateTexture
        public LUT3D(int size) : base(size) {
        }

        protected override void CreateTexture(int size) {
            Texture3D = Static.CreateTexture3D(new Vector3Int(size, size, size), GraphicsFormat.R32G32B32A32_SFloat, false);
        }
        public override void SetData(Vector4[] cells, bool apply = true) {
            Texture3D.SetPixelData(cells);
            if (apply) {
                Texture3D.Apply();
            }
        }
        public override Vector4[] GetData() {
            var data = Texture3D.GetPixelData<Vector4>(0);
            return data.ToArray();
        }
        public override void DestroyTexture() {
            if (Texture3D) {
                GameObject.DestroyImmediate(Texture3D);
            }
        }

        public static LUT3D? CreateFromCubeFileContent(string content, bool apply = true) {
            return ParseLUT3D(content, (size) => new LUT3D(size));
        }
    }

}
