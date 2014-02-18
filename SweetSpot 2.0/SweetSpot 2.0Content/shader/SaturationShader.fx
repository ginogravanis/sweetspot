sampler textureSampler;
float elapsedTime;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(textureSampler, texCoord);
	float blackWhite = dot(color.rgb, float3(0.3, 0.59, 0.11));
	return lerp(color, blackWhite, effectIntensity);
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
