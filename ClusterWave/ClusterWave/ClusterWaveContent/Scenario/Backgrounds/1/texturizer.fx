float4x4 View;
float4x4 Projection;
float time;

texture light;
sampler samp = sampler_state{ Texture = light; };

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Coord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Color : COLOR0;
	float2 Coord : TEXCOORD0;
};

VertexShaderOutput VS(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(mul(input.Position, View), Projection);

	output.Color = input.Color;
	output.Coord = input.Position * 0.3;

    return output;
}

float4 PS(VertexShaderOutput input) : COLOR0
{
	float2 coords = input.Coord * 10;
	coords.xy += time;
	coords.x += sin(time + coords.y * 0.25) * 0.5;
	coords.y += cos(time + coords.x * 0.25) * 0.5;
	return tex2D(samp, coords) * input.Color;
}

technique Technique1
{
    pass Pass1
    {
        VertexShader = compile vs_2_0 VS();
        PixelShader = compile ps_2_0 PS();
    }
}
