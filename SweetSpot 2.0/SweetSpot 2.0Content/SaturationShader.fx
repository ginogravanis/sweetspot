sampler textureSampler;
float effectAmount;

float4 Saturation(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(textureSampler, texCoord);
	float blackWhite = dot(color.rgb, float3(0.3, 0.59, 0.11));
	return lerp(color, blackWhite, effectAmount);
}

technique SaturationShader
{
    pass Default
    {
        PixelShader = compile ps_2_0 Saturation();
    }
}
