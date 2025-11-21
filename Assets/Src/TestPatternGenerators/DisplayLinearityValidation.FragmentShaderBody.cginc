int3 pixelPosition = int3(input.position.xy, 0);

int cellSize = 128;
int border = 32;
uint2 cellIndex2D = uint2(pixelPosition.xy) / cellSize;
 
uint2 innerCellPosition = uint2(pixelPosition.xy) % cellSize;
static const int numOptions = 4;
static const float3 aOptions[numOptions] = {1.0.xxx, 0.5.xxx, 0.25.xxx, 1.0.xxx};
static const float3 bOptions[numOptions] = {0.0.xxx, 0.0.xxx, 0.0.xxx, 0.5.xxx};

static const float3 multiplierOptions[numOptions] = {float3(1,1,1), float3(1,0,0), float3(0,1,0), float3(0,0,1)};
float3 multiplier = multiplierOptions[cellIndex2D.y % numOptions];
float3 a = aOptions[cellIndex2D.x % numOptions] * multiplier;
float3 b = bOptions[cellIndex2D.x % numOptions] * multiplier;

float3 m = 0.5 * (a+b);

if (any(innerCellPosition< border) || any(innerCellPosition>=(cellSize - border))) {
    return float4(m, 1);
}

int checker = (pixelPosition.x + pixelPosition.y) % 2;

return float4(lerp(a,b, checker), 1);


