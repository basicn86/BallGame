shader_type spatial;

uniform sampler2D noise_tex : source_color;

uniform float wave_horizontal_scale = 0.02;
varying float wave_height;

void vertex() {
	vec2 uv = vec2(
    VERTEX.x * wave_horizontal_scale + TIME * 0.03,
    VERTEX.z * wave_horizontal_scale + TIME * 0.02
	);
	
	VERTEX.y += texture(noise_tex, uv).r * 2.0;
	wave_height = VERTEX.y;
}

void fragment() {
	// Calculate partial derivatives for flat shading
    vec3 dx = dFdx(VERTEX);
    vec3 dz = dFdy(VERTEX);
    NORMAL = -normalize(cross(dx, dz));
	
	ROUGHNESS = 0.0;
	float foam = smoothstep(1.9, 1.92, wave_height);
	ALBEDO = mix(vec3(0.0, 0.2, 0.5), vec3(0.5, 0.5, 0.5), foam);
	
	float fresnel = pow(1.0 - dot(NORMAL, VIEW), 3.0);
	ALBEDO += fresnel * 0.1;
}


//void light() {
//	// Called for every pixel for every light affecting the material.
//	// Uncomment to replace the default light processing function with this one.
//}
