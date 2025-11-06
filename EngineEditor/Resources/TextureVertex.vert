#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec4 aColor;
layout (location = 2) in vec2 aTexCoord;

layout(std140, binding = 0) uniform Camera
{
    mat4 aProjection;
    mat4 aView;
};

out vec4 vColor;
out vec2 vTexCoord;

void main()
{
    vColor = aColor;
    vTexCoord = aTexCoord;
    
    gl_Position = aProjection * aView * vec4(aPosition, 1.0);
}