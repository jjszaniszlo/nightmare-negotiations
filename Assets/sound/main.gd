extends Node3D

@onready var RSound: AudioStreamPlayer = $AirRaid


func _on_hurt_sound_pressed() -> void:
	$HitHurt.play()

var sound_pos = 0.0
func _on_button_pressed() -> void:
	if RSound.playing:
		sound_pos = RSound.get_playback_position()
		RSound.stop()
		$AirButton.text = "resume"
	else:
		RSound.play(sound_pos)
		$AirButton.text = "pause"

	
