#version 450

in vec4 vColor;
in vec2 vTexCoord;

uniform sampler2D vTexture;

out vec4 FragColor;

void main()
{
    
    FragColor = texture(vTexture, vTexCoord) * vColor;
}