shader_type canvas_item;

uniform float Opacity = 0.0f;

void fragment()
{
    vec4 col = texture(TEXTURE,UV).rgba;
	col.r = 240.0;
	col.g = 0.0;
	col.b = 0.0;
	col.a = Opacity;
    COLOR=col;
}