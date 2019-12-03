/*Info:
This is an example of blending without using alpha channel.

Src: https://stackoverflow.com/questions/726549/algorithm-for-additive-color-mixing-for-rgb-values
*/

#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform vec3 tintClr;

void main ()
{
    outputColor = texture(texture0, texCoord);

	outputColor.r = min(outputColor.r + tintClr.r, 255);
	outputColor.g = min(outputColor.g + tintClr.g, 255);
	outputColor.b = min(outputColor.b + tintClr.b, 255);
}