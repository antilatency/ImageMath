int2 position = input.position.xy;

uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
//return float4(position<0,0,1);

float3 pixels[dctN][dctN];
for (int x = -dctR; x <= dctR; x++) {
    for (int y = -dctR; y <= dctR; y++) {
        int2 offset = int2(x, y);
        int2 p = position + offset;
        p = max(p, 0);
        p = min(p, textureSize - 1);
        float4 color = Texture.Load(int3(p, 0));
        pixels[x + dctR][y + dctR] = color.rgb;
    }
}
float3 center = pixels[dctR][dctR];

float3 maxAbsCoefficient = 0;
for (int u = 0; u < dctN; u+=2) {
    for (int v = 0; v < dctN; v+=2) {
        float3 coefficient = CalculateCoefficient(pixels, u, v);
        maxAbsCoefficient = max(maxAbsCoefficient, abs(coefficient));
    }
}


float3 reconstructed = 0;
for (int u = 0; u < dctN; u+=2) {
    for (int v = 0; v < dctN; v+=2) {
        float3 coefficient = CalculateCoefficient(pixels, u, v);
        float basis = DCTBasis(dctR,dctR, u, v);

        //float nu = 1-u/(N-1);
        //float nv = 1-v/(N-1);
        float m = (u+v) / ((dctN-1)+(dctN-1));

        //float m = 1;
        if (u>0 || v>0) {
            float3 s = sign(coefficient);
            coefficient *= s;
            coefficient -= Level;
            coefficient = max(coefficient, 0.0);
            coefficient *= s;
        }

        reconstructed += coefficient * basis;
    }
}
#ifdef RenderDelta
return float4((reconstructed - center), 1);
#else
return float4(reconstructed, 1);
#endif


