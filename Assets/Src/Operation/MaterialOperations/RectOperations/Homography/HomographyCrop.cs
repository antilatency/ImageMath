using System;

using UnityEngine;
#nullable enable
namespace ImageMath {

    [FilePath]
    public partial record HomographyCrop : TextureOperation {


        public Matrix4x4 HomographyMatrix { get; private set; } = Matrix4x4.identity;

        public HomographyCrop(Homography.SolverDelegate solverDelegate, Texture texture, Vector2[] srcCorners, Vector2[]? destCorners = null) : base(texture) {
            if (srcCorners.Length != 4) {
                throw new System.Exception("Source corners must have exactly 4 points.");
            }

            if (destCorners == null) {
                destCorners = Homography.DefaultDestinationCorners;
            }
            else if (destCorners.Length != 4) {
                throw new System.Exception("Destination corners must have exactly 4 points.");
            }

            HomographyMatrix = Homography.Calculate(destCorners, srcCorners, solverDelegate);

        }

#if UNITY_EDITOR        
        public static string GetFragmentShaderBody() => Embed("HomographyCrop.FragmentShaderBody.cginc");
#endif


    }


    
}
