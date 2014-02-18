sampler textureSampler;
float elapsedTime;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(textureSampler, texCoord);
	color.rgb /= color.a;
	color.rgb = ((color.rgb - 0.5f) * max(1 - effectIntensity, 0)) + 0.5f;
	color.rgb *= color.a;
	return color;
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
