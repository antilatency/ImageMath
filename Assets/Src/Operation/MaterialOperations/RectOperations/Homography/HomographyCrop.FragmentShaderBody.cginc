float3 uv_ = mul((float3x3)HomographyMatrix, float3(input.uv,1));
float2 uv = uv_.xy / uv_.z;
float4 result = Texture.Sample(samplerTexture, input.uv);
return result;