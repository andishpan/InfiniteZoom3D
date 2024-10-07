extends Control

var button_type = null
var player
#var main

#func _ready():
	#main = get_parent()
	#$FadeTransition/AnimationPlayer.play("fade_out")

func _on_play_pressed():
	#get_tree().change_scene_to_file("res://src/main/main.tscn")
	button_type = "play"
	var main = get_node_or_null("/root/Main")
	if main != null:
		main.get_node("SceneTransition/CameraSway").show()
		main.get_node("SceneTransition/CameraSway/PlayerContainer").activateShield(1, true)
	SpeedController.reset()
	SpeedController.activate()
	GameState.set_GAME_STATE(1)
	hide()
	#$FadeTransition.show()
	#$FadeTransition/fade_timer.start()
	#$FadeTransition/AnimationPlayer.play("fade_in")

func _on_settings_pressed():
	#get_tree().change_scene_to_file("res://src/mushroom/infinite_muspshroom.tscn")
	button_type = "settings"
	GameState.set_GAME_STATE(3)
	hide()
	#$FadeTransition.show()
	#$FadeTransition/fade_timer.start()
	#$FadeTransition/AnimationPlayer.play("fade_in")

#func _on_fade_timer_timeout():
	#if button_type == "play":
		#get_tree().change_scene_to_file("res://src/main/main.tscn")
	#elif button_type == "settings":
		#get_tree().change_scene_to_file("res://src/mushroom/infinite_mushroom.tscn")
