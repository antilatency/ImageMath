//hlsl include to apply a 3D LUT to a color
// This shader code applies a 3D LUT (Look-Up Table) to a color input.
// The LUT is a 3D texture that maps input colors to output colors based on the LUT's data.
// The LUT is sampled using the input color's RGB values, which are transformed into 3D texture coordinates.
// The resulting color is then returned as the output.
// The LUT is expected to be a 3D texture with dimensions that are a power of two.

//get size of LUT
uint sizeX;
uint sizeY;
uint sizeZ;

LUT.GetDimensions(sizeX, sizeY, sizeZ);
float3 size = float3(sizeX, sizeY, sizeZ);

//inverse lerp define
#define InverseLerp(a, b, t) ((t - a) / (b - a))

float3 uvw = InverseLerp(DomainMin, DomainMax, inputColor.rgb);
uvw = clamp(uvw, 0, 1);

float3 invSize = 1.0 / size;
float3 offset = 0.5 * invSize;
float3 scale = 1 - invSize;

uvw = uvw * scale + offset;

float4 color = LUT.SampleLevel(samplerLUT, uvw, 0);

return color;
