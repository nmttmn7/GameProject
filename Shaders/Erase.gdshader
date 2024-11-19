shader_type canvas_item;

// --- Uniforms --- //
uniform float percentage: hint_range(0.0, 1.0, 0.01) = 1.0;
uniform float angle: hint_range(0.0, 6.28318530718, 0.1308996939) = 0.0;
uniform float line_width: hint_range(0.0, 1.0, 0.01) = 0.02;
uniform vec4 line_color: source_color = vec4(1.0);

// --- Functions --- //
vec2 rotate(vec2 _pos, float _sine, float _cosine) {
	return vec2(_pos.x * _cosine + _pos.y * -_sine, _pos.x * _sine + _pos.y * _cosine);
}

void fragment() {
	float sine = sin(angle);
	float cosine = cos(angle);
	float len = abs(sine) + abs(cosine);
	float dist = (rotate(((UV) - vec2(0.5)) / len, sine, cosine).y + 0.5) * (1.0 - line_width);
	float line = step(percentage, dist);
	COLOR.a -= line;
	COLOR.rgb = mix(COLOR.rgb, line_color.rgb, step(percentage, dist + line_width));
}