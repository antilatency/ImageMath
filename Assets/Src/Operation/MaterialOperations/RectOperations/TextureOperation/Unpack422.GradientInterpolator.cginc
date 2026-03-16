float GradientInterpolator(float left, float right, float center, float w) {
    float vx  = right - left;
    float vx2 = vx * vx;

    float hw  = 0.5 * w;

    float sl  = vx2 + 1.0;
    float isl = 1.0 / sl;

    float num = hw + isl * (vx * (center - left) + hw);
    float det = w + (w + vx2) * isl;
    if (abs(det) < 1e-7) {
        return 0.5;
    }
    return num / det;
}