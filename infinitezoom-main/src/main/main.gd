extends Node3D

func _ready():
	$FadeTransition/AnimationPlayer.play("fade_out")
