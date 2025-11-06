#version 450

layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;

layout(std140, binding = 0) uniform Camera
{
    mat4 aProjection;
    mat4 aView;
};

out vec3 vColor;

void main()
{
    vColor = aColor;
    
    gl_Position = aProjection * aView * vec4(aPosition, 1.0);
}