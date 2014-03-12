sampler textureSampler;
sampler textureSampler2;
float elapsedTime;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	int maxPixelSize = 100;
	float dx = (maxPixelSize * effectIntensity) * (1./width);
	float dy = (maxPixelSize * effectIntensity) * (1./height);
	float2 pos = float2(dx * floor(texCoord.x/dx),
						dy * floor(texCoord.y/dy));
	return tex2D(textureSampler, pos);
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
