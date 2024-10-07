extends Control

var heart_size: int = 640
# Called when the node enters the scene tree for the first time.
func _ready():	
	var _gui_node = get_parent()
	#gui_node.life_changed.connect(_on_gui_life_changed)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass


func _on_gui_life_changed(player_hearts):
	var heart_node = $Heart
	heart_node.set_size(Vector2(player_hearts * heart_size, heart_size))
