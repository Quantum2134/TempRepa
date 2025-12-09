#version 450 core

in vec4 v_Color;
in vec2 v_uv;

out vec4 Color;

uniform sampler2D v_texture;

void main()
{
	Color = texture(v_texture, v_uv) * v_Color;
}