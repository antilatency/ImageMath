// Hash 2D → 1D (good quality)
float Hash21(float2 p){
    p = frac(p * float2(123.34, 345.45));
    p += dot(p, p + 34.345);
    return frac(p.x * p.y);
}

float GaussianNoise(float2 uv){
    // Two independent uniform randoms in (0,1)
    float u1 = Hash21(uv);
    float u2 = Hash21(uv + 37.0);   // shifted uv to decorrelate

    // Avoid log(0)
    u1 = max(u1, 1e-6);

    // Box–Muller transform → Gaussian(mean=0, stddev=1)
    float r = sqrt(-2.0 * log(u1));
    float a = 6.2831853 * u2;       // 2π*u2

    return r * cos(a);
}

float4 GaussianNoiseRGBA(float2 uv){
    return float4(
        GaussianNoise(uv + float2(0.0, 0.0)),
        GaussianNoise(uv + float2(12.34, 56.78)),
        GaussianNoise(uv + float2(90.12, 34.56)),
        GaussianNoise(uv + float2(78.90, 12.34))
    );
}