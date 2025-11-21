uint2 textureSize = 0;


uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
uint2 halfTextureSize = textureSize / 2;


int2 pixelPosition = input.position.xy;




float r = Texture.Load(int3(pixelPosition.x , pixelPosition.y + halfTextureSize.y, 0)).r;
float2 gb = float2(
    Texture.Load(int3(pixelPosition.x / 2, pixelPosition.y, 0)).r,
    Texture.Load(int3(pixelPosition.x / 2 + halfTextureSize.x, pixelPosition.y, 0)).r
);

#if Algorithm_NearestNeighbor
return float4(r, gb, 1.0);
#else

bool oddX = (pixelPosition.x % 2) == 1;
bool evenX = (pixelPosition.x % 2) == 0;

if (evenX){
    return float4(r, gb, 1.0);
}

int nextX = min(pixelPosition.x / 2 + 1, halfTextureSize.x -1);
float2 gbNext = float2(
    Texture.Load(int3(nextX, pixelPosition.y, 0)).r,
    Texture.Load(int3(nextX + halfTextureSize.x, pixelPosition.y, 0)).r
);

#if Algorithm_LinearInterpolation
return float4(r, (gb + gbNext) * 0.5, 1.0);
#else

/*float2 uv = input.uv;

float r = Texture.Sample(sampler_Linear_Clamp, uv * float2(1.0, 0.5) + float2(0.0, 0.5)).r;
float g = Texture.Sample(sampler_Linear_Clamp, uv * float2(0.5, 0.5)).r;
float b = Texture.Sample(sampler_Linear_Clamp, uv * float2(0.5, 0.5) + float2(0.5, 0.0)).r;

return float4(r, g, b, 1.0);*/

float leftR = Texture.Load(int3(pixelPosition.x -1 , pixelPosition.y + halfTextureSize.y, 0)).r;
float rightR = Texture.Load(int3(pixelPosition.x +1 , pixelPosition.y + halfTextureSize.y, 0)).r;

float dR = rightR - leftR;
float absR = abs(dR);
if (absR < 0.0001){
    float2 gbAvg = (gb + gbNext) * 0.5;
    return float4(r, gbAvg, 1.0);
}
float t = (r - leftR) / dR;
t = clamp(t, 0.0, 1.0);
float2 absGB = abs(gbNext - gb);

//return float4(absR, absGB, 1);
float2 sum = absR + absGB;
float2 proj = absGB / sum;
//return float4(proj, 0.0, 1.0);
float2 tSuppressed = lerp(t, 0.5, pow(proj, Power));
float2 myGB = lerp(gb, gbNext, tSuppressed);
return float4(r, myGB, 1.0);



/*
int clasterIndex = (pixelPosition.x + 1) / 2 - 1;


float rs[4];
int ry = pixelPosition.y + halfTextureSize.y;
for (int i = 0; i < 4; i++){
    int rx = clamp( clasterIndex * 2 + i, 0, int(textureSize.x) -1);
    rs[i] = Texture.Load(int3(rx, ry, 0)).r;
}

float myR = rs[pixelPosition.x - clasterIndex * 2];

float2 gbs[2];
int gby = pixelPosition.y;
for (int i = 0; i < 2; i++){
    int gbx = clamp(clasterIndex + i, 0, int(halfTextureSize.x) -1);
    float g = Texture.Load(int3(gbx, gby, 0)).r;
    float b = Texture.Load(int3(gbx + halfTextureSize.x, gby, 0)).r;
    gbs[i] = float2(g, b);
}

float avRs[2];
avRs[0] = (rs[0] + rs[1]) * 0.5;
avRs[1] = (rs[2] + rs[3]) * 0.5;
//where is myR between avRs[0] and avRs[1]?

float linearT = pixelPosition.x % 2 == 0 ?  0.75 : 0.25;


float d = avRs[1] - avRs[0];
float absR = abs(d);
//return abs(d);
if (absR < 0.0001){
    float2 gb = lerp(gbs[0], gbs[1], linearT);
    return float4(myR, gb.x, gb.y, 1.0);
}

float t =  (myR - avRs[0]) / d;


t = clamp(t, 0.0, 1.0);

float2 absGB = abs(gbs[1] - gbs[0]);

//return float4(absR, absGB, 1);

float2 sum = absR + absGB;
float2 proj = absGB / sum;

//return float4(proj, 0.0, 1.0);

float2 tSuppressed = lerp(t, linearT, pow(proj, Power));

float2 myGB = lerp(gbs[0], gbs[1], tSuppressed);

return float4(myR, myGB.x, myGB.y, 1.0);*/

#endif
#endif //Algorithm_NearestNeighbor
