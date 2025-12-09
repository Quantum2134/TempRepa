#version 450

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec4 a_Color;


layout(std140, binding = 0) uniform Camera
{
	mat4 View;
	mat4 Projection;
};

out vec4 v_Color;

void main()
{
	v_Color = a_Color;

	gl_Position = View * Projection * vec4(a_Position, 1.0);
}
