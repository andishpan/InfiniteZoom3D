extends Node3D

var levels = []
var zoom_level = 1.0
var wireframe_enabled = true
var elapsed_time = 0.0
var transition_time = 10.0
var color_change_interval = 0.5 
var color_elapsed_time = 0.0

var camera
var zoom_speed = 1.02
var max_cubes = 10

const SHADER_PATH = "res://assets/shaders/zoom_wireframe_cube.gdshader"

func _ready():
	RenderingServer.set_debug_generate_wireframes(true)
	get_viewport().debug_draw = Viewport.DEBUG_DRAW_WIREFRAME

	var world_environment = WorldEnvironment.new()
	add_child(world_environment)

	var environment = Environment.new()
	environment.background_mode = Environment.BG_COLOR
	environment.background_color = Color(0, 0, 0)
	
	environment.glow_enabled = true
	environment.glow_intensity = 0.8
	environment.glow_bloom = true
	
	world_environment.environment = environment

	var cube_container = get_node("CubeContainer")
	
	for i in range(max_cubes):
		var size = pow(2, (i % 10) + 1) 
		var new_cube = create_cube(size, cube_container)
		new_cube.scale = Vector3(pow(0.1, (i % 10) + 1), pow(0.1, (i % 10) + 1), pow(0.1, (i % 10) + 1))
		levels.append(new_cube)

	camera = Camera3D.new()
	camera.name = "Camera"
	camera.fov = 75
	camera.current = true
	#add_child(camera)
	#camera.global_transform.origin = Vector3(0, 0, 1)
	
	set_process(true)

func create_cube(size, parent):
	var cube = MeshInstance3D.new()
	var mesh = BoxMesh.new()
	mesh.size = Vector3(size, size, size)
	cube.mesh = mesh
	cube.position = Vector3(0, 0, -20)
	
	var shader = load(SHADER_PATH)
	var shader_material = ShaderMaterial.new()
	shader_material.shader = shader
	shader_material.set_shader_parameter("color", Color(randf(), randf(), randf()))
	
	cube.material_override = shader_material
	parent.add_child(cube)
	return cube

func _process(_delta):
	var delta: float = SpeedController.delta
	zoom_level *= zoom_speed
	
	for index in range(len(levels)):
		var new_scale = pow(0.1, (index % 10) + 1) * zoom_level * delta
		levels[index].scale = Vector3(new_scale, new_scale, new_scale)
	
	for cube in levels:
		cube.rotation_degrees += Vector3(1, 1, 0) * _delta * 60
		if(cube.rotation_degrees.x >= 360): cube.rotation_degrees -= Vector3(1, 1, 0) * 360
	
	color_elapsed_time += _delta
	if color_elapsed_time >= color_change_interval:
		change_cube_colors()
		color_elapsed_time = 0.0
	
	if zoom_level > pow(10, 5.6):
		reset_scene()

func change_cube_colors():
	for cube in levels:
		var material = cube.material_override
		if material is ShaderMaterial:
			material.set_shader_parameter("color", Color(randf(), randf(), randf()))

func reset_scene():
	zoom_level = pow(10, 4.9)
	#camera.global_transform.origin = Vector3(0, 0, 10)
	for i in range(max_cubes):
		#var size = pow(2, (i % 10) + 1) 
		levels[i].scale = Vector3(pow(0.1, (i % 10) + 1), pow(0.1, (i % 10) + 1), pow(0.1, (i % 10) + 1))
		#levels[i].rotation_degrees = Vector3.ZERO  

func _input(event):
	if event is InputEventKey and event.pressed and event.keycode == Key.KEY_P:
		var vp = get_viewport()
		if wireframe_enabled:
			vp.debug_draw = Viewport.DEBUG_DRAW_DISABLED
		else:
			vp.debug_draw = Viewport.DEBUG_DRAW_WIREFRAME
		wireframe_enabled = !wireframe_enabled
