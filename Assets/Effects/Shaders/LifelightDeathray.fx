sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

float time;
float3 mainColor;
matrix worldViewProjection;

struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(in VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput) 0;
    float4 pos = mul(input.Position, worldViewProjection);
    output.Position = pos;
    
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;

    return output;
}

float InverseLerp(float from, float to, float x)
{
    return saturate((x - from) / (to - from));
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;
    
    float bloomFadeout = sin(3.1415 * coords.y);
    float4 baseTexture = (tex2D(uImage1, float2(frac(coords.x * 4.3 - time * 3), coords.y)).r + bloomFadeout) * color;
    
    float noise = tex2D(uImage2, float2(frac(coords.x * 5 - time * 3.5), coords.y)).r;
    float4 noiseTexture = InverseLerp(0.4, 0.5, pow(noise * bloomFadeout, 1.2)) * float4(mainColor, 1);
    
    float opacity = 1;
    
    if (coords.x < 0.05)
        opacity *= pow(coords.x / 0.05, 6);
    if (coords.x > 0.95)
        opacity *= pow(1 - (coords.x - 0.95) / 0.05, 6);
    
    opacity *= bloomFadeout;
    
    return baseTexture * 1.33 * opacity + noiseTexture * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}