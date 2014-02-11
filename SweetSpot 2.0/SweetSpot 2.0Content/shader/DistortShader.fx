sampler textureSampler;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
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
