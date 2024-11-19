extends Node


# Called when the node enters the scene tree for the first time.
func _ready():	
	SceneManager.fade_in()



# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass



func _on_pressed():
	SceneManager.change_scene("res://Scenes/TEST.tscn", { "speed": 2 })
	print("WOWOOWOW");
