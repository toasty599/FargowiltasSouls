sampler mainImage : register(s0);
sampler noiseImage : register(s1);
sampler colorImage : register(s2);

float opacity;
float time;
float2 resolution;
float3 lighting;

float2 ToRotationVector2(float f)
{
    return float2(sin(f + 1.57), sin(f));
}

float4 Wings(float2 uv : TEXCOORD0) : COLOR0
{
    float4 basePixel = tex2D(mainImage, uv);
    if (basePixel.r != 1 && basePixel.g != 1 && basePixel.b != 1)
        return basePixel;
    
    // Pixelate.
    uv.x -= uv.x % (1 / (resolution.x * 2));
    uv.y -= uv.y % (1 / (resolution.y * 2));

    float3 noise = tex2D(noiseImage, float2(frac(uv.x *.9 + time * 0.3), uv.y * 0.5 + time * 0.2)).rgb;
    float rotation = noise.r * 9.42477796;
    float2 offset = ToRotationVector2(rotation);
    float3 result = tex2D(noiseImage, uv * 0.63 + offset * 0.06);
    
    float3 noise2 = tex2D(colorImage, float2(frac(0.6 - time * 0.5), time * 0.2 + length(uv) * 0.5)).rgb + (result * 0.3);
    //float3 result = noise * 0.5 + noise2 * 0.5;
    return float4(noise2 * lighting, 1);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 Wings();
    }
}