shader_type canvas_item;

//horizontal noise
uniform sampler2D noise1 : repeat_enable;
uniform sampler2D noise2 : repeat_enable;
uniform vec2 scroll1 = vec2(0.1,0.);
uniform vec2 scroll2 = vec2(-0.1,0.);
uniform float distortion_strength : hint_range(-1,1) = 0;
//color tint variables
uniform float tint_alpha : hint_range(0.0, 1.0, 0.1) = 0.3;
uniform float tint_beta : hint_range(0.0, 1.0, 0.1) = 0.3;
uniform float tint_gamma : hint_range(0.0, 1.0, 0.1) = 0.34;

uniform sampler2D gradient : source_color;

float rgb2v(vec4 c){
	float param_sum = tint_alpha + tint_beta + tint_gamma;

	float v = (tint_alpha * c.r + tint_beta *  c.g + tint_gamma * c.g) / param_sum;
	return v;
}
void fragment() {
	// Called for every pixel the material is visible on.
	float noise1x = texture(noise1, UV + scroll1 * TIME).r;
	float noise2x = texture(noise1, UV + scroll2 * TIME).r;
	float wobbly_noise = noise1x * noise2x;

	vec2 screen_uv = UV + distortion_strength * vec2(wobbly_noise,0);
	vec4 screen_color = texture(TEXTURE,screen_uv);

	float color_bw = rgb2v(screen_color);
	vec4 sampled_color = texture(gradient, vec2(color_bw, 0.));
	sampled_color.a = screen_color.a;
	COLOR = sampled_color;

}
