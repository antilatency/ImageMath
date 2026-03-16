int3 pixelPosition = int3(input.position.xy, 0);
uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);


float3 values[9];
int index = 0;
for (int x = -1; x <= 1; x++){
    for (int y = -1; y <= 1; y++){
        int3 samplePosition = pixelPosition + int3(x, y, 0);
        samplePosition = clamp(samplePosition, int3(0, 0, 0), int3(textureSize - 1, levels - 1));
        values[index] = Texture.Load(samplePosition).rgb;
        index++;
    }
}

/*float3 dxx = values[7] - 2.0 * values[4] + values[1];
float3 dyy = values[5] - 2.0 * values[4] + values[3];
float3 dxy = 0.25 * (values[8] - values[6] - values[2] + values[0]);
float3 trace = dxx + dyy;
float3 delta = sqrt((dxx - dyy)*(dxx - dyy) + 4.0 * dxy * dxy);

float3 lambda1 = 0.5 * (trace + delta);
float3 lambda2 = 0.5 * (trace - delta);

// maximum curvature magnitude
float3 maxCurvature = max(abs(lambda1), abs(lambda2));
return float4(maxCurvature * Multiplier * 0.3, 1.0);*/

float3 average = float3(0.0, 0.0, 0.0);
for (int i = 0; i < 9; i++){
    average += values[i];
}
average /= 9.0;

float3 maxDif = 0;
for (int i = 0; i < 9; i++){
    maxDif = max(maxDif, abs(values[i] - average));
}

float3 current = values[4];
float3 diff = abs(current - average);

return float4(abs(maxDif)* Multiplier, 1.0);
