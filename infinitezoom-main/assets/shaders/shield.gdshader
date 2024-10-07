shader_type spatial;
render_mode cull_disabled;

uniform float shield_uptime;


void fragment() {
	vec3 p = VERTEX - CAMERA_POSITION_WORLD;
	float breathe = sin(TIME * 2.0) * 0.5 + 0.5;
	ALPHA = (0.6 * mod(floor((p.y * 1.5 + TIME * 3.0) * 1.1), 2)
		- (1.0 - breathe) * 0.5)
		* -0.1 * log2(exp2(-shield_uptime * 27.4)) + exp2(-10);
	float s =  sin(-p.y + p.z + TIME * 2.0) * 0.5 + 0.5;
	ALBEDO = vec3(0.3 * s, 0.1 + 0.34 * (1.0 - s), 0.69) + vec3(breathe) * 0.15;
}
