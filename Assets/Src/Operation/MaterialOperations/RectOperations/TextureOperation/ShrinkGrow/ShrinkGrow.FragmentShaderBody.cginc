

uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
float2 pixelsToUV = 1.0 / float2(textureSize.x, textureSize.y);

float4 result = 0;
int count = 0;
//blur 3x3 pixels
for (int x = -1; x <= 1; x++) {
    for (int y = -1; y <= 1; y++) {
        float2 offset = int2(x, y) * pixelsToUV;
        float2 uv = input.uv + offset;

        if (any(uv>1) || any(uv<0)) continue;
        count++;
        result += Texture.Sample(samplerTexture, uv);
    }
}

return result / count;

/*
float4 color = Texture.SampleLevel(samplerImageMath_T0, input.uv, levels-1);
color /= color.a;


for (int i = levels-2; i >= 0; i--){
    float4 nextColor = Texture.SampleLevel(samplerImageMath_T0, input.uv, i);
    color.a = nextColor.a;
    if (nextColor.a < Epsilon) continue;
    
    float a = pow(nextColor.a,Power);
    float aCorrection = pow(nextColor.a,Power-1);

    float4 newColor = nextColor*aCorrection + color * (1.0 - a);
    color.rgb = newColor.rgb;                
}

return color;*/