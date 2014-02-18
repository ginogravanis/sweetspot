sampler textureSampler;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	texCoord.y = texCoord.y + 0.05 * effectIntensity * sin(texCoord.y * 100);
	float4 color = tex2D(textureSampler, texCoord);
	return color;
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
