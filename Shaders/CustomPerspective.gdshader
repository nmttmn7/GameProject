// Hey this is Hei! This shader "fakes" a 3D-camera perspective on CanvasItems.
// License: MIT

shader_type canvas_item;

// Camera FOV
group_uniforms perspective;
uniform float fov : hint_range(1, 179) = 90;
uniform bool cull_back = true;
uniform float y_rot : hint_range(-180, 180) = 0.0;
uniform float x_rot : hint_range(-180, 180) = 0.0;
// At 0, the image retains its size when unrotated.
// At 1, the image is resized so that it can do a full
// rotation without clipping inside its rect.
uniform float inset : hint_range(0, 1) = 0.0;
group_uniforms;
// Consider changing this to a uniform and changing it from code

varying flat vec2 o;
varying vec3 p;



// Creates rotation matrix
void vertex(){
	float sin_b = sin(y_rot / 180.0 * PI);
	float cos_b = cos(y_rot / 180.0 * PI);
	float sin_c = sin(x_rot / 180.0 * PI);
	float cos_c = cos(x_rot / 180.0 * PI);

	mat3 inv_rot_mat;
	inv_rot_mat[0][0] = cos_b;
	inv_rot_mat[0][1] = 0.0;
	inv_rot_mat[0][2] = -sin_b;

	inv_rot_mat[1][0] = sin_b * sin_c;
	inv_rot_mat[1][1] = cos_c;
	inv_rot_mat[1][2] = cos_b * sin_c;

	inv_rot_mat[2][0] = sin_b * cos_c;
	inv_rot_mat[2][1] = -sin_c;
	inv_rot_mat[2][2] = cos_b * cos_c;


	float t = tan(fov / 360.0 * PI);
	p = inv_rot_mat * vec3((UV - 0.5), 0.5 / t);
	float v = (0.5 / t) + 0.5;
	p.xy *= v * inv_rot_mat[2].z;
	o = v * inv_rot_mat[2].xy;


	VERTEX += (UV - 0.5) / TEXTURE_PIXEL_SIZE * t * (1.0 - inset);
}

group_uniforms shine;
uniform bool shine = true;
uniform vec4 shine_color: source_color = vec4(1.0, 1.0, 1.0, 0.25);

uniform float line_width: hint_range(0.0, 2.0, 0.01) = 0.1;
uniform float angle: hint_range(0.0, 6.28318530718, 0.1308996939) = 0.785398163397;

uniform float speed: hint_range(0.0, 10.0, 0.1) = 1.0;
uniform float wait_cycles: hint_range(0.0, 10.0, 0.1) = 1.0;
group_uniforms;
// --- Functions --- //
vec2 rotate_precalculated(vec2 _pos, float _sine, float _cosine) {
	return vec2(_pos.x * _cosine + _pos.y * -_sine, _pos.x * _sine + _pos.y * _cosine);
}



group_uniforms pulse;
uniform vec4 pulse_color : source_color = vec4(1.0); //Shine color
uniform float progress : hint_range(0, 1) = 0;
group_uniforms;


// --- Uniforms --- //
group_uniforms dissolve;
uniform float percentage: hint_range(0.0, 1.0, 0.01) = 1.0;
uniform sampler2D burn_texture;

uniform vec4 layer_1: source_color = vec4(0.2, 0.2, 0.2, 1.0);
uniform float size_1 = 0.05;

uniform vec4 layer_2: source_color = vec4(1.0, 0.0, 0.0, 1.0);
uniform float size_2 = 0.05;

uniform vec4 layer_3: source_color = vec4(1.0, 0.5, 0.0, 1.0);
uniform float size_3 = 0.05;
group_uniforms;
// --- Functions --- //




void fragment(){
	if (cull_back && p.z <= 0.0) discard;
	vec2 uv = (p.xy / p.z).xy - o;
    COLOR = texture(TEXTURE, uv + 0.5);
	COLOR.a *= step(max(abs(uv.x), abs(uv.y)), 0.5);


	// NewShader
	if(shine){
	float sine = sin(angle);
	float cosine = cos(angle);
	float len = 1.5 - max(abs(sine), abs(cosine)) + line_width;
	float line = smoothstep(-line_width, line_width,
			rotate_precalculated((UV - vec2(0.5)), sine, cosine).y - mod(TIME * speed, (len * 2.0) * wait_cycles) + len);
	COLOR.rgb += shine_color.rgb * shine_color.a * vec3(line * (1.0 - line) * 4.0);
	}

	//Pulse
	//	COLOR.rgb += pulse_color.rgb;
	//	COLOR.a *= pulse_color.a;
	COLOR.rgb = mix(COLOR.rgb, pulse_color.rgb, progress * pulse_color.a);


	float noise = texture(burn_texture, UV).r * (1.0 - (size_1 + size_2 + size_3 + 0.01));

	COLOR.a -= step(percentage, noise);
	COLOR.rgb = mix(COLOR.rgb, layer_3.rgb, step(percentage, noise + size_1 + size_2 + size_3));
	COLOR.rgb = mix(COLOR.rgb, layer_2.rgb, step(percentage, noise + size_1 + size_2));
	COLOR.rgb = mix(COLOR.rgb, layer_1.rgb, step(percentage, noise + size_1));


}