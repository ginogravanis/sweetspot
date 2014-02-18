sampler textureSampler;
float elapsedTime;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	texCoord.x = texCoord.x + 0.1 * effectIntensity * sin(texCoord.y * 100);
	texCoord.y = texCoord.y + 0.1 * effectIntensity * sin(texCoord.y * 100);
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
