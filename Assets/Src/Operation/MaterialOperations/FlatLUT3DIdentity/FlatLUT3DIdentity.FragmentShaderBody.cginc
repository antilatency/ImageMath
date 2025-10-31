int2 index2d = int2(input.position.xy);
uint2 textureSize = ImageMath_RenderTargetSize;

int index1d = index2d.x + index2d.y * textureSize.x;
int cubeSize = (int)round(pow(textureSize.x * textureSize.y, 1.0 / 3.0));
uint3 index3d = uint3(
    index1d % cubeSize,
    (index1d / cubeSize) % cubeSize,
    index1d / (cubeSize * cubeSize)
);


float3 value = index3d / (cubeSize - 1.0);

return float4(value, 1.0);


