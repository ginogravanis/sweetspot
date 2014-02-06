sampler textureSampler;
float effectAmount;

float4 Brightness(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(textureSampler, texCoord);
	float black = dot(color.rgb, float3(0, 0, 0));
	return lerp(color, black, effectAmount);
}

technique BrightnessShader
{
    pass Default
    {
        PixelShader = compile ps_2_0 Brightness();
    }
}
