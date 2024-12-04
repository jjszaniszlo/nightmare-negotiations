shader_type canvas_item;

uniform vec4 line_color : source_color = vec4(1.0, 1.0, 1.0, 1.0); 
uniform vec4 scan_color : source_color = vec4(0.0, 1.0, 0.0, 0.5);  
uniform vec4 blip_color : source_color = vec4(1.0, 0.0, 0.0, 1.0);  
uniform float range_lines : hint_range(0.0, 10.0, 1.0) = 4.0;  
uniform float sector_lines : hint_range(0.0, 10.0, 1.0) = 4.0; 
uniform float line_width : hint_range(0.01, 0.1) = 0.01;
uniform float rotation_speed : hint_range(0.1, 5.0) = 1.0; 
uniform float scan_width : hint_range(0.1, 1.0) = 0.05;  
uniform float blip_size : hint_range(0.01, 0.1) = 0.05;  
uniform vec2 monster_positions[10];  

void fragment() {
    vec2 uv = UV * 2.0 - 1.0;

    float distance_from_center = length(uv);  
    if (distance_from_center > 1.0) {
        discard; 
    }
    float angle = atan(uv.y, uv.x);

    float line = mod(distance_from_center, 1.0 / range_lines);
    float sector = mod(abs(angle), 1.0 / sector_lines);

    float line_condition = step(1.0 - line_width, line);
    float sector_condition = step(1.0 - (line_width / distance_from_center), sector);

    vec3 color = mix(line_color.rgb, line_color.rgb, min(line_condition, sector_condition));

    float scan_angle = mod(TIME * rotation_speed , 2.0 * PI);
    float scan = abs(mod(angle / (2.0 * PI), 1.0) - scan_angle);
    if (scan < scan_width) {
        color = mix(color, scan_color.rgb, scan_color.a);
    }

    // Add monster blips
    for (int i = 0; i < 10; i++) {
        vec2 monster_pos = monster_positions[i]; 
        float blip_distance_squared = dot(monster_pos - uv, monster_pos - uv); 

    }
    COLOR.rgb = color;
    COLOR.a = 1.0; 
}
