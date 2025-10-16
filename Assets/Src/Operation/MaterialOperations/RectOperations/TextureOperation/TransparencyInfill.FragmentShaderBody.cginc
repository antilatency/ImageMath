uint2 textureSize = 0;
uint levels = 0;
Texture.GetDimensions(0, textureSize.x, textureSize.y, levels);

float4 color = Texture.SampleLevel(sampler_Linear_Clamp, input.uv, levels-1);
color /= color.a;


for (int i = levels-2; i >= 0; i--){
    float4 nextColor = Texture.SampleLevel(sampler_Linear_Clamp, input.uv, i);
    color.a = nextColor.a;
    if (nextColor.a < Epsilon) continue;
    
    float a = pow(nextColor.a,Power);
    float aCorrection = pow(nextColor.a,Power-1);

    float4 newColor = nextColor*aCorrection + color * (1.0 - a);
    color.rgb = newColor.rgb;                
}

return color;