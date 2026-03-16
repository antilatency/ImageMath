int3 pixelPosition = int3(input.position.xy, 0);
uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);

/*int minX = max(0, pixelPosition.x - 1);
int maxX = min(int(textureSize.x) - 1, pixelPosition.x + 1);
int minY = max(0, pixelPosition.y - 1);
int maxY = min(int(textureSize.y) - 1, pixelPosition.y + 1);

float3 referenceValue = Texture.Load(int3(minX, minY, 0)).rgb;*/
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

bool3 clipping = bool3(true, true, true);
for (int i = 1; i < 9; i++){
    bool3 equal = values[0] == values[i];
    clipping = clipping && equal;
}

return float4(clipping * values[0], 1.0);


