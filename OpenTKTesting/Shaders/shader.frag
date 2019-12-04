/*Info:
This is an example of alpha blending.

Src: https://stackoverflow.com/questions/726549/algorithm-for-additive-color-mixing-for-rgb-values
*/

#version 330 core

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform vec4 u_tintClr;

void main ()
{
    vec4 fragClr = texture(texture0, texCoord);
	
	if (fragClr.a > 0)
	{
		outputColor.a = 1.0 - (1.0 - u_tintClr.a) * (1.0 - fragClr.a); // 0.75
		outputColor.r = u_tintClr.r * u_tintClr.a / outputColor.a + fragClr.r * fragClr.a * (1.0 - u_tintClr.a) / outputColor.a; // 0.67
		outputColor.g = u_tintClr.g * u_tintClr.a / outputColor.a + fragClr.g * fragClr.a * (1.0 - u_tintClr.a) / outputColor.a; // 0.33
		outputColor.b = u_tintClr.b * u_tintClr.a / outputColor.a + fragClr.b * fragClr.a * (1.0 - u_tintClr.a) / outputColor.a; // 0.00
	}
}