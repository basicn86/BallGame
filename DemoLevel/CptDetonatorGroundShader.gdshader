shader_type spatial;

uniform sampler2D Texture;
uniform sampler2D BurnedTexture;

const int MaxBurnPoints = 1024;
uniform uint BurnPointCount = 0u;

uniform vec3 BurnPoints[MaxBurnPoints];

varying vec3 world_pos;

void vertex() {
	vec4 view_pos = MODELVIEW_MATRIX * vec4(VERTEX, 1.0);
	world_pos = (INV_VIEW_MATRIX * view_pos).xyz;
}

void fragment() {
	ALBEDO = texture(Texture, UV).xyz;

	float lowDist = 100000.0;
	for(uint i = 0u; i < BurnPointCount; i++){
		float dist = distance(BurnPoints[i], world_pos);
		if(lowDist > dist) lowDist = dist;
	}

	if(lowDist < 3.0f) {
		float blendFactor = lowDist / 3.0f;
		ALBEDO = mix(texture(BurnedTexture, UV * 32.0).xyz, texture(Texture, UV).xyz, blendFactor);
	}
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
