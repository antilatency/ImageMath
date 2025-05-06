float2 ndc = input.uv * 2 - 1;
float r = length(ndc);
float alpha = 1;
if (r > 1) {
    ndc /= r;
    r = 1;
    alpha = 0;
}
float z = sqrt(1 - r * r);
float3 normal = float3(ndc, -z);

float d = dot(normal, -LightDirection);
return float4(d * Color,alpha);