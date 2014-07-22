sampler textureSampler;
float s;
float c;
float target = 0.7;

float4 main(float2 texCoord: TEXCOORD0) : COLOR
{
	float targetSize = lerp(target, 1.0, texCoord.y);
	float offset = (1.0 - targetSize) * 0.5;
	float stretch = 1.0 / targetSize;
	texCoord.x = stretch * (texCoord.x - offset);

	float2 newCoords;
	newCoords.x = ((texCoord.x - 0.5) * c) + ((texCoord.y - 0.5) * (-s)) + 0.5;
	newCoords.y = ((texCoord.x - 0.5) * s) + ((texCoord.y - 0.5) * c) + 0.5;
	texCoord = newCoords;
	
	return tex2D(textureSampler, texCoord);
}

float2x2 RotationMatrix(float rotation)
{
	float c = cos(rotation);
	float s = sin(rotation);
	return float2x2(c, -s, s, c);
}

technique Shader
{
    pass Default
    {
        PixelShader = compile ps_2_0 main();
    }
}
