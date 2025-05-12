using UnityEngine;
namespace ImageMath{
    [FilePath]
    public partial record ConvexHullSelect : ColorTransformOperation{
        
        [DynamicArray(64)]
        public Vector4[] Planes {get; set;} = null;

        public static Vector4 PlaneToVector4(Plane plane) {
            return new Vector4(plane.normal.x, plane.normal.y, plane.normal.z, plane.distance);
        }

        public ConvexHullSelect(Texture texture, Plane[] planes) : base(texture) {

            Planes = new Vector4[planes.Length];
            for (int i = 0; i < planes.Length; i++) {
                Planes[i] = PlaneToVector4(planes[i]);
            }
        }
        public ConvexHullSelect() : base() { }
        public static string GetColorTransform() {
            return @"float3 color = inputColor.rgb;
float maxDistance = -1;
float4 closestPlane = float4(1,0,0,0);
for (int p = 0; p < Planes_Size; p++){
    float4 plane = Planes[p];
    float distance = dot(plane.xyz, color) + plane.w;
    if (distance > 0){
        return 0;
    }                 
}
return float4(color, 1);";
        }
    }


}