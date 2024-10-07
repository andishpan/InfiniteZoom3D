shader_type spatial;

uniform float thickness : hint_range(1.0, 10.0, 0.1) = 2.0;

void vertex() {
    // Called for every vertex the material is visible on.
}

void fragment() {
    // Speed of the scrolling effect
    float scrollSpeed = 10.0;
    // Current time modulo scrollSpeed to create a looping timer
    float scrollTimer = mod(TIME, scrollSpeed);
    // Calculate the scrolling offset based on screen UV coordinates and timer
    float scrollOffset = mod(SCREEN_UV.x + (scrollTimer / scrollSpeed), 1.0);
    
    // Create patterns for uneven and even stripes
    float unevenPass = ceil(max(mod(scrollOffset * 10.0, 2.0) - 1.0, 0));
    float evenPass = ceil(max(mod(scrollOffset * 10.0 - 1.0, 2.0) - 1.0, 0));
    
    // Calculate gradient values for scrolling effect
    float leftRightGradient = mod(scrollOffset * 10.0, 1.0);
    float rightLeftGradient = mod(10.0 - (scrollOffset * 10.0), 1.0);
    
    // Define colors for the stripes
    vec3 red = vec3(1.0, 0.0, 0.0);
    vec3 blue = vec3(0.0, 0.0, 1.0);
    
    // Apply the gradient to the colors based on the stripe patterns
    vec3 redLeft = red * leftRightGradient * unevenPass;
    vec3 redRight = red * rightLeftGradient * evenPass;
    vec3 blueLeft = blue * leftRightGradient * evenPass;
    vec3 blueRight = blue * rightLeftGradient * unevenPass;
    
    // Combine the colors to create the final output color
    vec3 color = redLeft + redRight + blueLeft + blueRight;

    // Wireframe thickness logic
    vec3 barycentric = VERTEX; // Assuming barycentric coordinates are stored in VERTEX
    vec3 edge_dist = fwidth(barycentric) * thickness; // Compute the edge distance based on thickness
    vec3 edge = smoothstep(vec3(0.0), edge_dist, barycentric);
    float edge_factor = min(min(edge.x, edge.y), edge.z);

    // Mix the wireframe color and the base color
    vec3 wireframe_color = mix(color, vec3(1.0), 1.0 - edge_factor); // Assuming white color for wireframe
    ALBEDO = wireframe_color;
}
