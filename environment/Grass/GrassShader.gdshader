shader_type spatial;
render_mode unshaded;
render_mode cull_disabled;
render_mode world_vertex_coords;


uniform vec3 color : source_color;
uniform vec3 tipColor : source_color;
uniform sampler2D waves : hint_normal;

varying vec3 instancePos;

void vertex() {
	instancePos = MODEL_MATRIX[3].xyz;
	
	
	if(VERTEX.y > 0.25 + instancePos.y){
		//VERTEX.x += sin(TIME * 5.0) / 3.0 * (instancePos.y - VERTEX.y);
		vec2 noise_uv = vec2(instancePos.x, instancePos.z) + vec2(TIME * 0.1);
		vec3 wave_offset = texture(waves, noise_uv).xyz;
		
		VERTEX.x += (wave_offset.x - 0.5) * 0.25;
		VERTEX.z += (wave_offset.y - 0.5) * 0.25;
	}
}

void fragment() {
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	
	// Normalize the Y value of the world position between min_height and max_height
    float factor = clamp((world_pos.y - instancePos.y) * 4.0, 0.0, 1.0);

    // Interpolate between the top and bottom colors based on the normalized Y value
    vec3 currentColor = mix(color, tipColor, factor);
	
	ALBEDO = currentColor;
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
