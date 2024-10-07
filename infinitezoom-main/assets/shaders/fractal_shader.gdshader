shader_type spatial;

uniform float zoom : hint_range(0.1, 10.0) = 1.0;
uniform vec3 center = vec3(0.0, 0.0, 0.0);
uniform vec3 base_color = vec3(0, 0, 1);
uniform int max_iter : hint_range(1, 100) = 50;

vec3 hsv2rgb(vec3 c)
{
    vec4 K = vec4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    vec3 p = abs(fract(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * mix(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

vec3 palette(float t) {
    vec3 a = base_color;
    vec3 b = vec3(0.5, 0.5, 0.5); // Amplitude of the color variations
    vec3 c = vec3(1.0, 1.0, 1.0); // Frequency of the color changes
    vec3 d = vec3(0.0, 0.33, 0.67); // Phase shift for each color channel
    return a + b * cos(6.28318 * (c * t + d));
}

void fragment() {
    mat4 inverseModelMatrix = inverse(MODEL_MATRIX);
    vec3 p = (inverseModelMatrix * vec4(VERTEX, 1.0)).xyz / zoom - center;
    vec4 z = vec4(p, 0.0);
    vec4 c = vec4(p, 0.0);

    int i = 0;
    for (; i < max_iter; i++) {
        if (dot(z, z) > 4.0) {
            break;
        }
        z = vec4(
            z.x * z.x - z.y * z.y - z.z * z.z - z.w * z.w,
            2.0 * z.x * z.y + 2.0 * z.z * z.w,
            2.0 * z.x * z.z - 2.0 * z.y * z.w,
            2.0 * z.x * z.w + 2.0 * z.y * z.z
        ) + c;
    }

    float color = float(i) / float(max_iter);
	vec3 rgb = vec3(color, color, color);
	
    ALBEDO = palette(color);
}
