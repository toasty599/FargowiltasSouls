sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float time;
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

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    float4 color = input.Color;
    float2 coords = input.TextureCoordinates;

    // Get the pixel of the fade map. What coords.x is being multiplied by determines
    // how many times the uImage1 is copied to cover the entirety of the prim. 2, 2
    float fadeMapBrightness = tex2D(uImage1, float2(frac(coords.x * 0.7 - time * 1.5), coords.y)).r;
    
    // You determine what this is by simply fucking around with changing stuff and seeing how it changes,
    // until you get something that looks cool.
    float bloomOpacity = lerp(pow(sin(coords.y * 3.141), lerp(2, 6, coords.x)), 0.7, coords.x);
    return color * pow(bloomOpacity, 5) * lerp(2, 6, fadeMapBrightness);
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
