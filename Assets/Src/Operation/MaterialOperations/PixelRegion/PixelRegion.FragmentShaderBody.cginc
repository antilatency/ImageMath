int3 pixelPosition = int3(input.position.xy, 0) + Offset;

#if ClampPixelCoordinates
uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
pixelPosition = clamp(pixelPosition, 0, int3(textureSize - 1, levels - 1));
#endif

float4 value = Texture.Load(pixelPosition);

return value;


