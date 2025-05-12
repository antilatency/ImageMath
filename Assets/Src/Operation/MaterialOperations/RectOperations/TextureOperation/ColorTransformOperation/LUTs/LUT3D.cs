using UnityEngine;
#nullable enable
namespace ImageMath {
    
    public class LUT3D : LUT3DBase<Texture3D> {
        public LUT3D(int size, string? title = null, Vector3? domainMin = null, Vector3? domainMax = null): base(size, title, domainMin, domainMax) {
        }

        protected override Texture3D CreateTexture(int size) {
            return Static.CreateTexture3DFloat4(new Vector3Int(size, size, size));
        } 
        
        public static LUT3D? CreateFromCubeFileContent(string content, bool apply = true) {
            return ParseLUT3D(content,
                (size, data) => {
                    var lut = new LUT3D(size, null, Vector3.zero, Vector3.one);
                    lut.SetData(data, apply);
                    return lut;
                });
        }        
    }

}
