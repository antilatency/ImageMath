//LUT Texture2D<float4>
//inputColor: float4
uint sizeX;
uint sizeY;
LUT.GetDimensions(sizeX, sizeY);

float3 color = inputColor.rgb;
float sum = color.r + color.g + color.b;
if (sum == 0)
    return float4(0,0,0,1);

float3 projected = color / sum;

float2 pixelPosition = projected.xy * float2(sizeX - 2, 2 * sizeY - 1);
int2 pixelPosition00 = int2(pixelPosition);
int2 pixelPosition10 = pixelPosition00 + int2(1, 0);
int2 pixelPosition01 = pixelPosition00 + int2(0, 1);

float2 frac = pixelPosition - pixelPosition00;
//return float4(frac,0,1);

if (frac.x + frac.y > 1) {
    frac = (1 - frac).yx;
    pixelPosition00 += int2(1,1);
}

if (pixelPosition00.y >= sizeY) {
    pixelPosition00 = int2(sizeX-1 ,2*sizeY-1) - pixelPosition00;
}
if (pixelPosition10.y >= sizeY) {
    pixelPosition10 = int2(sizeX-1 ,2*sizeY-1) - pixelPosition10;
}
if (pixelPosition01.y >= sizeY) {
    pixelPosition01 = int2(sizeX-1 ,2*sizeY-1) - pixelPosition01;
}

float3 transformed00 = LUT.Load(int3(pixelPosition00, 0)).rgb * sum;
float3 transformed10 = LUT.Load(int3(pixelPosition10, 0)).rgb * sum;
float3 transformed01 = LUT.Load(int3(pixelPosition01, 0)).rgb * sum;


return float4(transformed00 + frac.x * (transformed10 - transformed00) + frac.y * (transformed01 - transformed00), 1);