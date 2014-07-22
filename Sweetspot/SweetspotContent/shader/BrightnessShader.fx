sampler textureSampler;
float elapsedTime;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(textureSampler, texCoord);
	float black = dot(color.rgb, float3(0, 0, 0));
	return lerp(color, black, effectIntensity);
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
