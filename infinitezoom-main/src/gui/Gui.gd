extends Control

signal life_changed(player_hearts)

var max_lives = 3
var lives_amount = max_lives
var time = 0
var player_is_invincible = false
var game_over: bool = false

@onready var score_value_text = $ScorePanel/ScoreValueText

func _ready():
	SpeedController.reset()
	SpeedController.activate() 

func _process(delta):
	if game_over:
		SpeedController.reset()
		SpeedController.deactivate()
		GameState.set_GAME_STATE(2)
		var main = get_node_or_null("/root/Main")
		if main != null:
			main.get_node("GameOver").show()
			main.get_node("GameOver")._ready()
			main.get_node("Menu/Label")._ready()
			main.get_node("SceneTransition/CameraSway").hide()
		lives_amount = 3
		life_changed.emit(lives_amount)
		game_over = false
		#get_tree().change_scene_to_file("res://src/game_over/game_over_zoom_cube_spawner.tscn")
	
	time += delta
	
	if time >= 0.1:
		time -= 0.1
		if GameState.GAME_STATE == 1: Score.current += 1
	
	score_value_text.text = str(Score.current)
	if GameState.GAME_STATE == 1: show()
	else: hide()

func _on_area_3d_area_entered(_area):
	if !player_is_invincible && GameState.GAME_STATE == 1:
		lives_amount -= 1  
		life_changed.emit(lives_amount)
		if lives_amount < 1 : 
			game_over = true
	elif (lives_amount>max_lives): lives_amount = max_lives

func shield_status_changed(shield_up):
	player_is_invincible = shield_up;

func _input(event):
	if event.is_action_pressed("toggle_fullscreen"):
		if DisplayServer.window_get_mode() == DisplayServer.WINDOW_MODE_EXCLUSIVE_FULLSCREEN:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_MAXIMIZED)
		else:
			DisplayServer.window_set_mode(DisplayServer.WINDOW_MODE_EXCLUSIVE_FULLSCREEN)
	
	if event.is_action_pressed("end_relax") && GameState.GAME_STATE == 3:
		GameState.set_GAME_STATE(0)
		var main = get_node_or_null("/root/Main")
		if main != null: main.get_node("Menu").show()
