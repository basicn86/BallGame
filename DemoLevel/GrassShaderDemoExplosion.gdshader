shader_type spatial;
render_mode unshaded;
render_mode cull_disabled;
render_mode world_vertex_coords;


uniform vec3 color : source_color;
uniform vec3 tipColor : source_color;
uniform vec2 windPoint;
uniform float windStrength;
uniform sampler2D waves : hint_normal;

varying vec3 instancePos;

void vertex() {
	instancePos = MODEL_MATRIX[3].xyz;


	if(VERTEX.y > 0.25 + instancePos.y){
		//VERTEX.x += sin(TIME * 5.0) / 3.0 * (instancePos.y - VERTEX.y);
		vec2 noise_uv = vec2(instancePos.x, instancePos.z) + vec2(TIME * 0.1);
		vec3 wave_offset = texture(waves, noise_uv).xyz - vec3(0.5);

		VERTEX.x += wave_offset.x * 0.25;
		VERTEX.z += wave_offset.y * 0.25;

		vec2 directionToCenter = normalize(vec2(VERTEX.x, VERTEX.z) - windPoint);
		directionToCenter *= windStrength;

		VERTEX.x += directionToCenter.x * (VERTEX.y - instancePos.y);
		VERTEX.z += directionToCenter.y * (VERTEX.y - instancePos.y);
		
		VERTEX.y -= (VERTEX.y - instancePos.y) * abs(windStrength) / 2.0; //height
	}
}

void fragment() {
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;

    float factor = clamp((world_pos.y - instancePos.y) * 4.0, 0.0, 1.0);

    vec3 currentColor = mix(color, tipColor, factor);

	ALBEDO = currentColor;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
