/*Info:
This is an example of alpha blending.

Src: https://stackoverflow.com/questions/726549/algorithm-for-additive-color-mixing-for-rgb-values
*/

#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;

void main ()
{
    vec4 fragClr = texture(texture0, texCoord);

	vec4 tintClr = vec4(1.0, 0.0, 0.0, 0.5);//Alpha 50% blend of red
	
	outputColor.a = 1.0 - (1.0 - tintClr.a) * (1.0 - fragClr.a); // 0.75
	outputColor.r = tintClr.r * tintClr.a / outputColor.a + fragClr.r * fragClr.a * (1.0 - tintClr.a) / outputColor.a; // 0.67
	outputColor.g = tintClr.g * tintClr.a / outputColor.a + fragClr.g * fragClr.a * (1.0 - tintClr.a) / outputColor.a; // 0.33
	outputColor.b = tintClr.b * tintClr.a / outputColor.a + fragClr.b * fragClr.a * (1.0 - tintClr.a) / outputColor.a; // 0.00
}