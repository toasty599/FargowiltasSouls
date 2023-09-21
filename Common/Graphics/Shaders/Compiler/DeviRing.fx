sampler uImage0 : register(s0);
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);

float time;
float opacity;
matrix worldViewProjection;

float stretchAmount;
float scrollSpeed;
bool reverseDirection;

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
    
    float adjustedXposition = (sin(((coords.x * 2) - 1) * 1.57079) + 1) * 2;
    
	// Get the fade map pixel.
    float4 fadeMapColor;
    if (reverseDirection)
        fadeMapColor = tex2D(uImage1, float2(frac(adjustedXposition * stretchAmount - time * scrollSpeed), coords.y));
    else
        fadeMapColor = tex2D(uImage1, float2(frac(adjustedXposition * stretchAmount + time * scrollSpeed), coords.y));
    
    // Calcuate the grayscale version of the pixel and use it as the opacity.
    float finalOpacity = fadeMapColor.r;
    
    // Fade out at the end of the streak.
    if (coords.x < 0.1)
        finalOpacity *= pow(coords.x / 0.1, 3);
    if (coords.x > 0.85)
        finalOpacity *= pow(1 - (coords.x - 0.85) / 0.15, 3);
    
    //opacity = sin(coords.x * 3.1415 + uTime) * 0.5 + 0.5;
    
    return color * finalOpacity * opacity;
}

technique Technique1
{
    pass AutoloadPass
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
