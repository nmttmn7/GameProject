extends Node2D

var time: float = 0.0
var sine_offset_mult: float = 0.2
@export var time_multiplier: float = 2.0
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time += delta
	for i in range(get_child_count()):
		var c: Button = get_child(i).get_child(1)
		var val: float = sin(i + (time * time_multiplier))
		#print("Child %d, sin(%d) = %f" % [i, i, val])
		c.position.y += val * sine_offset_mult
