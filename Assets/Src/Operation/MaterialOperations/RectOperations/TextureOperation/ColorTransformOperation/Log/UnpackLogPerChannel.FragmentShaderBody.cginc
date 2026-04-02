float3 x = Texture.Sample(samplerTexture, input.uv).rgb;

float3 range = WhiteLevel - BlackLevel;
x = (x - BlackLevel) / max(range, Epsilon);

float3 exponent = ExponentScale * range;
float3 divider = pow(2, exponent) - 1;
// avoid division by zero when exponent ~ 0 (linear passthrough)
divider = max(abs(divider), Epsilon) * sign(divider + Epsilon);

x = (pow(2, x * exponent) - 1) / divider;

return float4(x, 1);
