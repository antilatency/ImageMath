int2 M = RenderTargetSize;
int2 N;
Texture.GetDimensions(N.x, N.y);
int2 outputIndex = input.position.xy;

int2 baseBlockSize = N / M;
int2 remainder = N % M;
int2 startIndex = outputIndex * baseBlockSize + min(outputIndex, remainder);

int2 blockSize = baseBlockSize + (outputIndex < remainder ? 1 : 0);
int2 maxBlockSize = baseBlockSize + (remainder>0);

int pixelsPerBlock = blockSize.x * blockSize.y;
float invPixelsPerBlock = 1.0f / pixelsPerBlock;

int maxPixelsPerBlock = maxBlockSize.x * maxBlockSize.y;
float invMaxPixelsPerBlock = 1.0f / maxPixelsPerBlock;


float4 a = Texture.Load(int3(startIndex, 0));
@GetInitialization

for (int i = 1; i < blockSize.x; i++) {
    int2 index = startIndex + int2(i, 0);
    float4 b = Texture.Load(int3(index, 0));
    {
        @GetOperation
    }
}
for (int i = 0; i < blockSize.x; i++) {
    for (int j = 1; j < blockSize.y; j++) {
        int2 index = startIndex + int2(i, j);
        float4 b = Texture.Load(int3(index, 0));
        {
            @GetOperation
        }
    }
}

@GetFinalization

return a;