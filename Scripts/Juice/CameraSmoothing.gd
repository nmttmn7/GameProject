extends Camera2D

const Dead_Zone = 530.0
var center: Vector2

func _ready() -> void:
	center = self.global_position;
	
func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		var _target = event.position;
		var differenceX = _target.x - center.x
		var differenceY = _target.y - center.y
		if(Dead_Zone < absf(differenceX)):
			if(differenceX < 0):
				self.global_position.x = _target.x + Dead_Zone + 40;
			else:
				self.global_position.x = _target.x - Dead_Zone + 40;
		
		var YDead_Zone = 278;
		if(YDead_Zone < absf(differenceY)):
			if(differenceY < 0):
				self.global_position.y = _target.y + YDead_Zone+ 40;
			else:
				self.global_position.y = _target.y - YDead_Zone+ 40;
	
		
