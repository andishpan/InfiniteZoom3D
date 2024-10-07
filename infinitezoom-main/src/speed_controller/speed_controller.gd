extends Node

const INITIAL_SPEED: float = 1
@export var INACTIVE_SPEED: float = INITIAL_SPEED

var delta: float = 0
var _speed: float = INITIAL_SPEED
var _active: bool = false

func _process(d):
	if _active and GameState.GAME_STATE == 1:
		delta = d * _speed
		_speed += d * 0.02
	else:
		delta = d * INACTIVE_SPEED

func reset():
	_speed = INITIAL_SPEED
	
func activate():
	_active = true

func deactivate():
	_active = false
