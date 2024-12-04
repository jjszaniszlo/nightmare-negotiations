shader_type canvas_item;

void fragment() {
	
    vec2 center = vec2(0.5, 0.5);
    float radius = 0.2; 

    float dist = distance(UV, center);

    if (dist <= radius) {
        COLOR = vec4(1.0, 0.0, 0.0, 1.0);  // red
    } else {
        discard; 
    }
}
