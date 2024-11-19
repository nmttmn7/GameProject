extends Node


var tween_expand: Tween
var time: float
@export var amplitude: float = 3.0
@export var frequency: float = 6.0
@export var offset: float = 0.0

@onready var card_texture: TextureRect = $TextureRect

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time += delta;
	wow()


func _on_button_button_down():
	if tween_expand and tween_expand.is_running():
		tween_expand.kill()
	tween_expand = create_tween().set_ease(Tween.EASE_IN).set_trans(Tween.TRANS_LINEAR)
	tween_expand.tween_property(self, "scale", Vector2(0.8, 1.5), 0.1)
	tween_expand.tween_property(self, "scale", Vector2(1.5, 0.8), 0.1)
	tween_expand.tween_property(self, "scale", Vector2(1, 1), 0.1)
	
func wow():
	print("ID ", get_instance_id())
	var s = sin(time * frequency) * amplitude + offset
	var c = cos(time * frequency) * amplitude + offset
	#var c = cos(time * speed)
	
	
	
	
	card_texture.material.set_shader_parameter("y_rot", s)
	card_texture.material.set_shader_parameter("x_rot", c)
	#card_texture.material.set_shader_parameter("y_rot", -rot_x)
	
