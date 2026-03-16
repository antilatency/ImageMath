//assuming target texture height is double source texture height

uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);

uint2 outputSize = outputDimensions(textureSize.x, textureSize.y);
int2 pixelPosition = input.position.xy;

uint2 p = sourcePosition(pixelPosition.x, pixelPosition.y, textureSize.x, textureSize.y);
uint component = sourceComponent(pixelPosition.x, pixelPosition.y, textureSize.x, textureSize.y);
float4 value = Texture.Load(int3(p.x, p.y, 0));
return value[component];

/*uint2 halfTextureSize = textureSize / 2;
uint2 pixelPosition = input.position.xy;


#if Layout_Cb0Y0Cr0Y1
int field = pixelPosition.x % 4;
int clasterIndex = pixelPosition.x / 4;
int leftPixelX = 2 * clasterIndex;
int rightPixelX = 2 * clasterIndex + 1;
switch (field) {
    case 0: {
        float y0 = Texture.Load(int3(leftPixelX, pixelPosition.y, 0)).r;
        return y0;
    }
    case 1: {
        float u = Texture.Load(int3(leftPixelX, pixelPosition.y, 0)).g;
        return u;
    }
    case 2: {
        float y1 = Texture.Load(int3(rightPixelX, pixelPosition.y, 0)).r;
        return y1;
    }
    case 3: {
        float v = Texture.Load(int3(leftPixelX, pixelPosition.y, 0)).b;
        return v;
    }
}
#endif*/


/*if (pixelPosition.y >= textureSize.y){
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
return g;*/
    

