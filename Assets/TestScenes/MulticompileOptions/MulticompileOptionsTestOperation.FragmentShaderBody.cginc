
#ifdef DrawCircle
    float2 center = float2(0.5, 0.5);
    float radius = 0.25;
    float2 uv = input.uv - center;
    if (length(uv) < radius) {
        return 0; // Inside the circle
    }
#endif

#ifdef Color_Red
    return float4(1,0,0,1); // Red color
#endif  

#ifdef Color_Green
    return float4(0,1,0,1); // Green color
#endif
#ifdef Color_Blue
    return float4(0,0,1,1); // Blue color
#endif

return 1;