shader_type spatial;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float scrollSpeed = 10.0;
	float scrollTimer = mod(TIME, scrollSpeed);
	float scrollOffset = mod(SCREEN_UV.x + (scrollTimer / scrollSpeed), 1);
	
	float unevenPass = ceil(max(mod(scrollOffset * 10.0, 2.0) - 1.0, 0));
	float evenPass = ceil(max(mod(scrollOffset * 10.0 - 1.0, 2.0) - 1.0, 0));
	float leftRightGradient = mod(scrollOffset * 10.0, 1.0);
	float rightLeftGradient = mod(10.0 - (scrollOffset * 10.0), 1.0);
	
	vec3 yellowLeft = vec3(1, 1, 0) * leftRightGradient * unevenPass;
	vec3 yellowRight = vec3(1, 1, 0) * rightLeftGradient * evenPass;
	vec3 purpleLeft = vec3(0.25, 0, 1) * leftRightGradient * evenPass;
	vec3 purpleRight = vec3(0.25, 0, 1) * rightLeftGradient * unevenPass;
	
	ALBEDO = (yellowLeft + yellowRight + purpleLeft + purpleRight);
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
