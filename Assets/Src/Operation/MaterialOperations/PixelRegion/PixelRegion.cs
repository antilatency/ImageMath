using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record PixelRegion: MaterialOperation {
        public Texture Texture { get; set; } = null;
        public Vector3Int Offset { get; set; } = Vector3Int.zero;
        
        [MulticompileOptions]
        public bool ClampPixelCoordinates { get; set; } = false;

        public PixelRegion(Texture texture, Vector2Int offset, int mip = 0) {
            Texture = texture;
            Offset = new Vector3Int(offset.x, offset.y, mip);
        }

        public PixelRegion() : base() { }

        public static string GetFragmentShaderBody() => IncludeOrEmbed($"{nameof(PixelRegion)}.FragmentShaderBody.cginc");
    }


}
