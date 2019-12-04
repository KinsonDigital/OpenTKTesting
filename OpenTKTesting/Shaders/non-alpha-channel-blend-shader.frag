/*Info:
This is an example of blending without using alpha channel.

Src: https://stackoverflow.com/questions/726549/algorithm-for-additive-color-mixing-for-rgb-values

This is additive blending
*/

#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main ()
{
    outputColor = texture(texture0, texCoord);

	vec4 tintClr = vec4(1.0, 0.0, 0.0, 0.5);
	
	outputColor.r = min(outputColor.r + tintClr.r, 255);
	outputColor.g = min(outputColor.g + tintClr.g, 255);
	outputColor.b = min(outputColor.b + tintClr.b, 255);
}