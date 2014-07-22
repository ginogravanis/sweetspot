sampler textureSampler;
float elapsedTime;
float effectIntensity;
int width;
int height;
float effectPeriod = 200;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	texCoord.x = texCoord.x + 0.03 * effectIntensity * cos(2.2 * 3.14159 * (elapsedTime / effectPeriod));
	texCoord.y = texCoord.y + 0.03 * effectIntensity * cos(1.7 * 3.14159 * (elapsedTime / effectPeriod));
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
