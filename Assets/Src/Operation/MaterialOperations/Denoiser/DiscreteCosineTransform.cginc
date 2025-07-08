float DCTBasis1D(int x, int u) {
    return dctBases[x * dctN + u];
}

float DCTBasis(int x, int y, int u, int v) {
    return DCTBasis1D(x, u) * DCTBasis1D(y, v);
}

float3 CalculateCoefficient(float3 pixels[dctN][dctN], int u, int v) {
    float3 sum = 0.0;
    for (int x = 0; x < dctN; x++) {
        for (int y = 0; y < dctN; y++) {
            sum += pixels[x][y] * DCTBasis(x, y, u, v);
        }
    }
    float au = (u == 0) ? dctIN : 2.0 * dctIN;
    float av = (v == 0) ? dctIN : 2.0 * dctIN;
    return sum * au * av;
}
