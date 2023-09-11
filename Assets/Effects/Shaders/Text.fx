sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float time;
float3 mainColor;
float3 secondaryColor;

float4 PulseShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 color = tex2D(uImage0, coords);
    float wave = frac(time + (coords.y));
    color.rgb *= (wave * mainColor) + ((1 - wave) * secondaryColor);
	

	return color * sampleColor * 2;

	//return color * tex2D(uImage0, coords).a;
}

float4 DiagonalShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float wave = frac(time / 2 + coords.y / 2 + coords.x / 2);
    color.rgb *= (wave * mainColor) + ((1 - wave) * secondaryColor);
	

    return color * sampleColor * 2;

	//return color * tex2D(uImage0, coords).a;
}

float4 CircleShader(float4 sampleColor : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(uImage0, coords);
    float dist = sqrt(pow((coords.x - 0.5), 2) + pow((coords.y - 0.5), 2));
    
    float wave = frac(time + dist);
    color.rgb *= (wave * mainColor) + ((1 - wave) * secondaryColor);
	

    return color * sampleColor * 2;

	//return color * tex2D(uImage0, coords).a;
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_2_0 PulseShader();
    }
	pass PulseUpwards
	{
		PixelShader = compile ps_2_0 PulseShader();
	}

    pass PulseDiagonal
    {
        PixelShader = compile ps_2_0 DiagonalShader();
    }

    pass PulseCircle
    {
        PixelShader = compile ps_2_0 CircleShader();
    }
}