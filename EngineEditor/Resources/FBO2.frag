#version 450

in vec3 vColor;
in vec2 vTexCoord;

uniform float time;
uniform sampler2D vTexture;

out vec4 FragColor;

float noise(vec2 p) 
{
    return fract(sin(dot(p, vec2(12.9898, 78.233))) * 43758.5453);
}

float fbm(vec2 p) {
    float total = 0.0;
    float amplitude = 0.5;
    for (int i = 0; i < 6; i++) {
        total += noise(p) * amplitude;
        p *= 2.0;
        amplitude *= 0.5;
    }
    return total;
}

mat2 rotate2D(float angle) {
    float s = sin(angle);
    float c = cos(angle);
    return mat2(c, -s, s, c);
}

vec3 stars(vec2 uv) {
    vec3 color = vec3(0.0);
    
    for (int i = 0; i < 50; i++) {
        vec2 starPos = vec2(noise(vec2(i)), noise(vec2(i+10)));
        float size = noise(vec2(i+20)) * 0.002 + 0.0005;
        float brightness = noise(vec2(i+30)) * 0.8 + 0.2;
        float dist = distance(uv, starPos);
        if (dist < size) {
            color += brightness * vec3(1.0, 0.9, 0.8);
        }
    }
    
    for (int i = 0; i < 10; i++) {
        vec2 starPos = vec2(noise(vec2(i+40)), noise(vec2(i+50)));
        float size = noise(vec2(i+60)) * 0.01 + 0.003;
        float dist = distance(uv, starPos);
        if (dist < size) {
            color += smoothstep(0.02, 0.0, dist/size) * vec3(1.0, 0.95, 0.9);
            
            vec2 d = uv - starPos;
            float angle = atan(d.y, d.x);
            float rays = sin(angle*8.0 + time*2.0);
            float rayIntensity = pow(1.0 - smoothstep(0.0, 0.2, dist/size), 4.0);
            color += max(0.0, rays) * rayIntensity * 0.5 * vec3(0.8, 0.9, 1.0);
        }
    }
    
    vec2 nebulaUV = uv * 3.0 + vec2(time*0.05);
    float nebula = fbm(nebulaUV);
    color += nebula * 0.2 * vec3(0.3, 0.4, 1.0);
    
    return color;
}

vec4 gravitationalLens(vec2 uv, vec2 center) {
    float dist = distance(uv, center);
    float distortion = 0.1 / (dist + 0.05);
    
    vec2 distortedUV = uv;
    vec2 dir = normalize(uv - center);
    distortedUV += dir * distortion * exp(-dist*10.0);
    
    vec4 color;
    color.r = texture(vTexture, distortedUV + vec2(0.003, 0.0)).r;
    color.g = texture(vTexture, distortedUV).g;
    color.b = texture(vTexture, distortedUV - vec2(0.003, 0.0)).b;
    color.a = 1.0;
    

    float blueShift = pow(1.0 - clamp(dist, 0.0, 1.0), 4.0);
    color.rgb = mix(color.rgb, color.bbb, blueShift*0.8);
    
    float eventHorizon = smoothstep(0.02, 0.01, dist);
    color.rgb = mix(color.rgb, vec3(0.0), eventHorizon);
    
    return color;
}

vec3 spaceRift(vec2 uv, vec2 center) {
    vec2 dir = uv - center;
    float angle = atan(dir.y, dir.x);
    float dist = length(dir);
    
    float spiral = sin(angle*10.0 - dist*20.0 + time*3.0);
    spiral = pow(max(0.0, spiral), 4.0);
    
    float energy = sin(dist*50.0 - time*5.0) * 0.5 + 0.5;
    energy *= smoothstep(0.3, 0.0, dist);
    
    vec3 color = vec3(0.0);
    color += spiral * vec3(0.2, 0.6, 1.0);
    color += energy * vec3(1.0, 0.3, 0.1);
    
    vec2 particleUV = rotate2D(-time*0.5) * dir * 5.0;
    particleUV += vec2(time*2.0);
    float particles = fbm(particleUV);
    particles = pow(particles, 4.0) * smoothstep(0.1, 0.0, dist);
    color += particles * vec3(1.0, 0.9, 0.5);
    
    return color;
}

void main()
{
    vec2 uv = vTexCoord;
    vec2 center = vec2(0.5, 0.5);
    
    vec3 color = stars(uv * 2.0 - 1.0);
    
    vec4 lensColor = gravitationalLens(uv, center);
    
    vec3 riftColor = spaceRift(uv, center);
    
    float riftIntensity = smoothstep(0.3, 0.0, distance(uv, center));
    color = mix(color, riftColor, riftIntensity);
    color = mix(color, lensColor.rgb, lensColor.a);
    
    vec3 sceneColor = texture(vTexture, uv).rgb;
    color += sceneColor * 0.2;
    
    color = pow(color, vec3(1.0/2.2)); 
    FragColor = vec4(color, 1.0);

    
}