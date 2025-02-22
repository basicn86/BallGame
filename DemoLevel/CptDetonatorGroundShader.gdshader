shader_type spatial;

uniform sampler2D Mask;
uniform sampler2D BurnedTexture;

float distanceSquared(vec3 a, vec3 b){
	vec3 diff = a - b;
	return dot(diff, diff);
}

void vertex() {

}

void fragment() {
	vec3 regularAlbedo = vec3(0.0,0.3,0.0);
	ALBEDO = regularAlbedo;
	
	float blendFactor = texture(Mask, UV).x;
	
	ALBEDO = mix(texture(BurnedTexture, UV * 32.0).xyz, regularAlbedo, blendFactor);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
