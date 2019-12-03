#version 330 core

out vec4 outputColor;

in vec2 texCoord;

// A sampler2d is the representation of a texture in a shader.
// Each sampler is bound to a texture unit (texture units are described in Texture.cs on the Use function)
// By default, the unit is 0, so no code-related setup is actually needed.
// Multiple samplers will be demonstrated in section 1.5
uniform sampler2D texture0;

void main ()
{
    // To use a texture, you call the texture() function.
    // It takes two parameters: the sampler to use, and a vec2, used as texture coordinates
    outputColor = texture(texture0, texCoord);//KEEP

	vec3 outputClrToMix = vec3(outputColor.r, outputColor.g, outputColor.b);

	vec3 mixClr = vec3(1, 0, 0);
	vec3 colorResult = mix(outputClrToMix, mixClr, 0.5);

	outputColor.r = colorResult.r;
	outputColor.g = colorResult.g;
	outputColor.b = colorResult.b;
}