sampler textureSampler;
float effectIntensity;
int width;
int height;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 color = tex2D(textureSampler, texCoord);
	float3x3 sepia = {0.393, 0.349, 0.272,
					  0.769, 0.686, 0.534,
					  0.189, 0.168, 0.131};
	float4 result;
	result.rgb = mul(color.rgb, sepia);
	result.a = 1.0f;
	return lerp(color, result, effectIntensity);
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
