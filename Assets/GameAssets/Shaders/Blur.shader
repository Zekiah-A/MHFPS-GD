shader_type canvas_item;

uniform float blur_amount : hint_range(0, 5);
uniform float translucency : hint_range(1, 4); //may need change

void fragment() {
	float texture_alpha = texture(TEXTURE, UV).a;
	COLOR = textureLod(SCREEN_TEXTURE, SCREEN_UV, blur_amount) * vec4(translucency, translucency, translucency, texture_alpha);
}