extends Node

# 0: main menu
# 1: gameplay
# 2: game over
# 3: relax mode

@export var GAME_STATE = 0

func set_GAME_STATE(state):
	GAME_STATE = state
