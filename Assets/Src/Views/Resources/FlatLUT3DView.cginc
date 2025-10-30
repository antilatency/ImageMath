        
Texture2D<float4> FlatLUT3D;
uint Size;


float3 GetValue(uint3 id, uint textureWidth){
    uint index = id.x + id.y * Size + id.z * Size * Size;
    int2 pixelCoordinate = int2(index % textureWidth, index / textureWidth);
    return FlatLUT3D.Load(int3(pixelCoordinate, 0)).xyz;
}
        
        

        