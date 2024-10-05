shader_type spatial;
render_mode cull_disabled; //allows both sides of the blade to be rendered
render_mode diffuse_toon;
render_mode specular_schlick_ggx;
render_mode world_vertex_coords; //simplifies the vertex shader so we don't need to account for rotation


uniform vec3 color : source_color;
uniform vec3 tipColor : source_color;
uniform sampler2D waves : hint_normal;

varying vec3 instancePos;

void vertex() {
	//this is used in the Fragment shader to determine where the base of the blade is
	instancePos = MODEL_MATRIX[3].xyz;
	
	
	if(VERTEX.y > 0.25 + instancePos.y){
		//VERTEX.x += sin(TIME * 5.0) / 3.0 * (instancePos.y - VERTEX.y);
		vec2 noise_uv = vec2(instancePos.x, instancePos.z) + vec2(TIME * 0.1);
		vec3 wave_offset = texture(waves, noise_uv).xyz - vec3(0.5);
		
		VERTEX.x += wave_offset.x * 0.25;
		VERTEX.z += wave_offset.y * 0.25;
	}
}

void fragment() {
	vec3 world_pos = (INV_VIEW_MATRIX * vec4(VERTEX, 1.0)).xyz;
	
    float factor = clamp((world_pos.y - instancePos.y) * 4.0, 0.0, 1.0); //magic numbers, probably need to fix later

    vec3 currentColor = mix(color, tipColor, factor);
	
	ALBEDO = currentColor;
	
	if(!FRONT_FACING){
		NORMAL = -NORMAL;
	}
}
