

float2 uv = input.uv;
float2 A = PointA;
float2 B = PointB;

float2 AB = B - A;
float2 ABPerp = normalize(float2(-AB.y, AB.x));
float2 AUV = uv - A;

float d = dot(AUV, ABPerp);
float dotAB = dot(AB, AB);

float t = dotAB<Epsilon ? 0 : dot(AUV, AB) / dotAB;
float distance = abs(d);
if (t<=0) distance = length(AUV);
if (t>=1) distance = length(uv - B);
if (distance>LineWidth) return 0;

distance /= LineWidth;
distance = LineSoftness<Epsilon ? distance>=1 : pow(distance, 1/LineSoftness);

float alpha = cos(distance * Pi) * 0.5 + 0.5;

return float4(Color.rgb,Color.a*alpha);