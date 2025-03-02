shader_type spatial;

uniform sampler2D Mask;
uniform sampler2D RegularTexture;
uniform sampler2D BurnedTexture;

void vertex() {

}

void fragment() {
	vec3 regularAlbedo = texture(RegularTexture, UV * 32.0).xyz * 0.5f;

	float blendFactor = texture(Mask, UV).x;

	ALBEDO = mix(texture(BurnedTexture, UV * 32.0).xyz, regularAlbedo, blendFactor);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
