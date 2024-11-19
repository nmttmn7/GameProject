extends Node2D

@export var max_offset: Vector2
@export var smoothing: float = 2.0

@export var parallax: Camera2D

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	var center: Vector2 = Vector2(-576,-324)
	var dist: Vector2 = get_global_mouse_position()
	var offset: Vector2 = dist/center
	
	var new_pos: Vector2
	
	new_pos.x = lerp(max_offset.x, -max_offset.x, offset.x)
	new_pos.y = lerp(max_offset.y, -max_offset.y, offset.y)

	parallax.position.x = lerp(parallax.position.x, new_pos.x, smoothing * delta)
	parallax.position.y = lerp(parallax.position.y, new_pos.y, smoothing * delta)
