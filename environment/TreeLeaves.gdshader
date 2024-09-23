shader_type spatial;

uniform vec3 albedo : source_color;

void vertex() {
	VERTEX.y = VERTEX.y + sin(TIME * 3.0 + VERTEX.x) * 0.1;
}

void fragment() {
	ALBEDO = albedo;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
