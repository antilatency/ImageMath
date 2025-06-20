int2 position = input.position.xy;
float4 center = Texture.Load(int3(position, 0));
if (Power == 0) {
    return center;
}

uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);



float4 sum = 0;
float4 sumOfSquares = 0;
int count = 0;
int size = Size;

for (int x = -size; x <= size; x++) {
    for (int y = -size; y <= size; y++) {
        float2 offset = int2(x, y);// * pixelsToUV;
        float2 p = position + offset;

        if (any(position >= textureSize) || any(position < 0)) continue;
        count++;
        float4 color = Texture.Load(int3(p,0));
        sum += color;
        sumOfSquares += color * color;
    }
}


float4 average = sum / count;
float4 dispersion = sumOfSquares / count - average * average;
float4 standardDeviation = sqrt(max(dispersion, 0.0));
float4 edges = saturate(2*standardDeviation / (Power*Power));

float4 correction = 1-edges;

float4 result = lerp(center, average, correction);
return result;