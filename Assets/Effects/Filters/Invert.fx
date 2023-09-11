sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float opacity;
float2 focusPosition;
float2 screenPosition;
float2 screenSize;

float4 Main(float2 coords : TEXCOORD0) : COLOR0
{
	float4 colour = tex2D(uImage0, coords);
    float2 comparePoint = (focusPosition - screenPosition) / screenSize;
    float threshold = opacity;
	
    float lerp1 = (opacity - 0.5) * 2;
	if (lerp1 < 0)
		lerp1 = 0;
	
	float lerp2 = 1 - lerp1;

	float averageColour = (colour.r + colour.g + colour.b) / 2; //comes before inversion to remember grayscale of original

	if (threshold >= 1 || distance(coords, comparePoint) < threshold) // If the shader has fully faded in OR the pixel is close enough
	{
		colour = float4(colour.a - colour.r, colour.a - colour.g, colour.a - colour.b, colour.a); // Invert
	}

	colour = float4(averageColour * lerp1 + colour.r * lerp2, averageColour * lerp1 + colour.g * lerp2, averageColour * lerp1 + colour.b * lerp2, colour.a); //grayscale
	
	return colour;
}

technique Technique1
{
    pass AutoloadPass
    {
		PixelShader = compile ps_2_0 Main();
	}
}