//assuming target texture height is double source texture height

uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);
uint2 halfTextureSize = textureSize / 2;
uint2 pixelPosition = input.position.xy;
/*
float2 uv = input.uv;

if (uv.y > 0.5){
    uv.y = (uv.y - 0.5) * 2.0;
    return Texture.Sample(samplerTexture, uv).r;
}

uv.y = uv.y * 2.0;
if (uv.x > 0.5){
    uv.x = (uv.x - 0.5) * 2.0;
    return Texture.Sample(samplerTexture, uv).b;
} else {
    uv.x = uv.x * 2.0;
    return Texture.Sample(samplerTexture, uv).g;
}*/

if (pixelPosition.y >= textureSize.y){
    float r = Texture.Load(int3(pixelPosition.x, pixelPosition.y - textureSize.y, 0)).r;
    return r;
}

if (pixelPosition.x >= halfTextureSize.x) {
        int x = 2 * (pixelPosition.x - halfTextureSize.x);
        float b = Texture.Load(int3(x, pixelPosition.y, 0)).b;
        return b;
}

int x = 2 * pixelPosition.x;
float g = Texture.Load(int3(x, pixelPosition.y, 0)).g;
return g;
    

