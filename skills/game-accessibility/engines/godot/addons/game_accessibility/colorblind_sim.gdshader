// DEVELOPER TOOL - colour-vision deficiency SIMULATION. NOT A PLAYER OPTION.
//
// Shows approximately how the screen may lose colour distinctions for dichromats, so you can find
// colour-only cues and token collisions while developing. It does not "correct" anything and must
// not be shipped as an accessibility setting; players get A11yColorTokens palettes plus symbols.
//
// Use: add a ColorRect covering the screen in a CanvasLayer drawn last, give it a ShaderMaterial
// with this shader, and pick `mode`. Remove it from release builds.
//
// Model: Machado, Oliveira & Fernandes 2009, severity 1.0 matrices (as tabulated by the MIT-licensed
// DaltonLens-Python reference). They map LINEAR RGB to simulated linear RGB, row by row, so the
// screen colour is decoded from sRGB exactly once, transformed with explicit dot products (no GLSL
// column-major ambiguity), clamped, and encoded back. Alpha is preserved.
// If your project renders 2D in linear space (rendering/viewport/hdr_2d), set source_is_linear.
shader_type canvas_item;

uniform sampler2D screen_texture : hint_screen_texture, filter_linear_mipmap;
// 0 = off, 1 = protanopia, 2 = deuteranopia, 3 = tritanopia.
uniform int mode : hint_range(0, 3) = 1;
uniform bool source_is_linear = false;

vec3 srgb_to_linear(vec3 c) {
	return mix(c / 12.92, pow((c + 0.055) / 1.055, vec3(2.4)), step(vec3(0.04045), c));
}

vec3 linear_to_srgb(vec3 c) {
	return mix(c * 12.92, 1.055 * pow(c, vec3(1.0 / 2.4)) - 0.055, step(vec3(0.0031308), c));
}

vec3 simulate(vec3 rgb, int m) {
	vec3 r0;
	vec3 r1;
	vec3 r2;
	if (m == 1) {
		r0 = vec3(0.152286, 1.052583, -0.204868);
		r1 = vec3(0.114503, 0.786281, 0.099216);
		r2 = vec3(-0.003882, -0.048116, 1.051998);
	} else if (m == 2) {
		r0 = vec3(0.367322, 0.860646, -0.227968);
		r1 = vec3(0.280085, 0.672501, 0.047413);
		r2 = vec3(-0.011820, 0.042940, 0.968881);
	} else if (m == 3) {
		r0 = vec3(1.255528, -0.076749, -0.178779);
		r1 = vec3(-0.078411, 0.930809, 0.147602);
		r2 = vec3(0.004733, 0.691367, 0.303900);
	} else {
		return rgb;
	}
	return vec3(dot(r0, rgb), dot(r1, rgb), dot(r2, rgb));
}

void fragment() {
	vec4 screen = textureLod(screen_texture, SCREEN_UV, 0.0);
	vec3 linear_rgb = source_is_linear ? screen.rgb : srgb_to_linear(screen.rgb);
	vec3 simulated = clamp(simulate(linear_rgb, mode), 0.0, 1.0);
	COLOR = vec4(source_is_linear ? simulated : linear_to_srgb(simulated), screen.a);
}
