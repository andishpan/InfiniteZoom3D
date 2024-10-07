extends Node3D

@export var parent_node: Node3D

var total_frames = 198  
var reset_frames = 100
var current_frame = 0


var start_position = Vector3(0, 0, 0)
var end_position = Vector3(0.09, -3.92, 0.00)
var start_rotation = Vector3(0, 0, 0)
var end_rotation = Vector3(0, 180, 0)
var start_scale = Vector3(1, 1, 1)
var end_scale = Vector3(6.400, 6.400, 6.400)

func _ready():
	$FadeTransition/AnimationPlayer.play("fade_out")
	var timer = Timer.new()
	timer.wait_time = 1.0 / 99.0
	timer.one_shot = false
	timer.connect("timeout", Callable(self, "_on_timer_timeout"))
	add_child(timer)
	timer.start()

func _on_timer_timeout():
	if current_frame > total_frames + reset_frames:
		current_frame = 0 
	
	var t = float(current_frame) / total_frames
	var reset_t = float(current_frame - total_frames) / reset_frames
	
	if current_frame <= total_frames:
		
		var new_position = start_position.lerp(end_position, t)
		parent_node.global_transform.origin = new_position
		
		
		var new_rotation_y = lerp(start_rotation.y, end_rotation.y, t)
		var new_rotation = Vector3(start_rotation.x, new_rotation_y, start_rotation.z)
		parent_node.rotation_degrees = new_rotation
		
		
		var new_scale = start_scale.lerp(end_scale, t)
		parent_node.scale = new_scale
	else:
		
		var new_position = end_position.lerp(start_position, reset_t)
		parent_node.global_transform.origin = new_position
		
		
		var new_rotation_y = lerp(end_rotation.y, start_rotation.y, reset_t)
		var new_rotation = Vector3(end_rotation.x, new_rotation_y, end_rotation.z)
		parent_node.rotation_degrees = new_rotation
		
		
		var new_scale = end_scale.lerp(start_scale, reset_t)
		parent_node.scale = new_scale

	
	print("Frame: ", current_frame)
	print("ParentNode Position: ", parent_node.global_transform.origin)
	print("ParentNode Rotation Degrees: ", parent_node.rotation_degrees)
	print("ParentNode Rotation Quaternion: ", parent_node.rotation)
	print("ParentNode Scale: ", parent_node.scale)
	print("-----------------------------")
	
	current_frame += 1
