#version 450

layout(location = 0) in vec3 a_Position;
layout(location = 1) in vec4 a_Color;
layout(location = 2) in vec2 a_uv;


layout(std140, binding = 0) uniform Camera
{
	mat4 View;
	mat4 Projection;
};

out vec4 v_Color;
out vec2 v_uv;

void main()
{
	v_Color = a_Color;
	v_uv = a_uv;

	gl_Position = View * Projection * vec4(a_Position, 1.0);
}
