sampler textureSampler;
sampler textureSampler2;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	int pixelSize = max(0, round(100 * effectIntensity));
	int x = round(texCoord.x * width);
	int y = round(texCoord.y * height);
	float2 samplePos = float2(1.0 * (x - (x % pixelSize)) / width,
							  1.0 * (y - (y % pixelSize)) / height);
	return tex2D(textureSampler, samplePos);
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
