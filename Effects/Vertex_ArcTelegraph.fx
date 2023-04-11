matrix WorldViewProjection;

struct VertexShaderInput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float2 TextureCoordinates : TEXCOORD0;
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
    VertexShaderOutput output;
    output.Position = mul(input.Position, WorldViewProjection);
    output.Color = input.Color;
    output.TextureCoordinates = input.TextureCoordinates;
    return output;
};

float4 MainPS(VertexShaderOutput input) : COLOR0
{
    float outlineMultiplier = 1;
    if(input.TextureCoordinates.x > 0.9f)
        outlineMultiplier = 2;
    return input.Color * input.TextureCoordinates.y * input.TextureCoordinates.y * 0.5f;
}


technique Technique1
{
    pass VertexTelegraphPass
    {
        VertexShader = compile vs_2_0 MainVS();
        PixelShader = compile ps_2_0 MainPS();

    }
}