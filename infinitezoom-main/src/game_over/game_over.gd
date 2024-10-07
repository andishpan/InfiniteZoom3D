extends Control

var button_type = null
@onready var score_value = $ScoreValue

func _ready():
	if Score.current > Score.best:
		Score.best = Score.current
	score_value.text = str(Score.current)

func _on_button_pressed():
	Score.current = 0
	GameState.set_GAME_STATE(0)
	hide()
	var main = get_node_or_null("/root/Main")
	if main != null: main.get_node("Menu").show()
	
	#get_tree().change_scene_to_file("res://src/main_menu/menu.tscn")
	#$FadeTransition.show()
	#$FadeTransition/fade_timer.start()
	#$FadeTransition/AnimationPlayer.play("fade_in")	

#func _on_fade_timer_timeout():
	#if button_type == "button":
		#get_tree().change_scene_to_file("res://src/main_menu/menu.tscn")
#get_tree().change_scene_to_file("res://src/main_menu/menu.tscn")
