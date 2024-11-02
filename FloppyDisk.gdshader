shader_type spatial;

uniform float Amplitude = 1.0f;
uniform float Speed = 1.0f;
uniform float Metallic = 0.0f;

uniform vec3 Albedo : source_color;

void vertex() {
	VERTEX.y += sin(TIME * Speed) * Amplitude;
}

void fragment() {
	ALBEDO = Albedo;
	METALLIC = Metallic;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
