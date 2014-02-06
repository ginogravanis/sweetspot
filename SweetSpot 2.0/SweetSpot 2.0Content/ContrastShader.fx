sampler textureSampler;
float effectAmount;

float4 Contrast(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(textureSampler, texCoord);
	color.rgb /= color.a;
	color.rgb = ((color.rgb - 0.5f) * max(1 - effectAmount, 0)) + 0.5f;
	color.rgb *= color.a;
	return color;
}

technique ContrastShader
{
    pass Default
    {
        PixelShader = compile ps_2_0 Contrast();
    }
}
