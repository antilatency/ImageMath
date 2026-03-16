uint2 textureSize = 0;

#define ReadProperty(propertyName) Texture.Load(int3(propertyName(pixelPosition.x, pixelPosition.y, outputSize.x, outputSize.y), 0)).r

uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);

uint2 outputSize = outputDimensions(textureSize.x, textureSize.y);
int2 pixelPosition = input.position.xy;
#if FlipVertically
pixelPosition.y = int(outputSize.y) - 1 - pixelPosition.y;
#endif

bool major = isMajor(pixelPosition.x, pixelPosition.y);
float Y;
float2 CbCr;

if (major){

    Y = ReadProperty(majorY);
    float Cb = ReadProperty(majorCb);
    float Cr = ReadProperty(majorCr);
    CbCr = float2(Cb, Cr);

} else { //minor pixel

    Y = ReadProperty(minorY);
    float CbLeft = ReadProperty(minorCbLeft);
    float CrLeft = ReadProperty(minorCrLeft);

    #if Algorithm_NearestNeighbor
    CbCr = float2(CbLeft, CrLeft);
    #else

    float CbRight = ReadProperty(minorCbRight);
    float CrRight = ReadProperty(minorCrRight);

    #if Algorithm_LinearInterpolation
    CbCr = lerp(float2(CbLeft, CrLeft), float2(CbRight, CrRight), 0.5);
    #else

    float YLeft = ReadProperty(minorYLeft);
    float YRight = ReadProperty(minorYRight);

    float t = GradientInterpolator(YLeft, YRight, Y, AntiNoiseProtection);
    t = clamp(t, 0, 1);

    CbCr = lerp(float2(CbLeft, CrLeft), float2(CbRight, CrRight), t);
    #endif
    #endif
}




float3 mul = 255.0 / float3(235.0 - 16.0, 240.0 - 16.0, 240.0 - 16.0);
float3 add = float3(-16.0/ 255.0, -16.0/ 255.0, -(16)/ 255.0);
float3 yuv = float3(Y, CbCr) + add;
yuv = yuv * mul;
return float4(yuv, 1.0);
