extends Button

@export var angle_x_max: float = 15.0
@export var angle_y_max: float = 15.0
@export var max_offset_shadow: float = 100.0
@export var min_offset_shadow: float = 10.0

@export_category("Oscillator")
@export var spring: float = 150.0
@export var damp: float = 10.0
@export var velocity_multiplier: float = 2.0

@export var grabbable: bool = true
var displacement: float = 0.0 
var oscillator_velocity: float = 0.0

var tween_flash: Tween
var tween_rot: Tween
var tween_hover: Tween
var tween_spawn: Tween
var tween_destroy: Tween
var tween_handle: Tween
var tween_return: Tween
var last_mouse_pos: Vector2
var mouse_velocity: Vector2
var following_mouse: bool = false
var last_pos: Vector2
var velocity: Vector2

@export var node2D: Node2D
@export var outline: ColorRect

@export var cardview : Node
@onready var card_texture: SubViewportContainer = $CardTexture
@onready var shadow: ColorRect = $Shadow
@onready var statusview: Node2D = $StatusView

@onready var popupContainer: VBoxContainer = $PopupContainer
var ogOrigin: Vector2 = position
#@onready var collision_shape = $DestroyArea/CollisionShape2D

@export var cardHitAnimation: PackedScene
var parentHand: Node2D
var main: Node2D

func _ready() -> void:
	# Convert to radians because lerp_angle is using that
	angle_x_max = deg_to_rad(angle_x_max)
	angle_y_max = deg_to_rad(angle_y_max)
	
	#cardview = get_parent().get_child(0)
	parentHand = get_parent().get_parent()
	main = parentHand.get_parent()
	#collision_shape.set_deferred("disabled", true)
	

func _process(delta: float) -> void:

	rotate_velocity(delta)
	follow_mouse(delta)
	handle_shadow(delta)
#	moveZ(delta)

var time: float = 0.0
@export var frequencyY: float = randf_range(0.5,2)
@export var ampY: float = randf_range(0.5,2)
@export var frequencyX: float = randf_range(0.5,2)
@export var ampX: float = randf_range(0.5,2)
var index: int = self.get_index()
var offset: float = 0.0




func moveZ(delta: float) -> void:
	time += delta
	
	var s = (sin(time * frequencyY + index) * ampY + offset);
	var c = (cos(time * frequencyX + index) * ampX + offset);
	
	card_texture.material.set_shader_parameter("y_rot",s)
	card_texture.material.set_shader_parameter("x_rot",c)


	
func idle(delta: float) -> void:
	time += delta
	
	var s = (sin(time * frequencyY + index) * ampY + offset) * 0;
	var c = (cos(time * frequencyX + index) * ampX + offset) * 0;
	
	card_texture.material.set_shader_parameter("y_rot",s)
	card_texture.material.set_shader_parameter("x_rot",c)
	#var rot_x: float = 0
	#var rot_y: float = 0
	
	#var rot_x: float = rad_to_deg(lerp_angle(-angle_x_max, angle_x_max, 25))
	#var rot_y: float = rad_to_deg(lerp_angle(angle_y_max, -angle_y_max, 25))
	#print("Rot x: ", rot_x)
	#print("Rot y: ", rot_y)
	
	#card_texture.material.set_shader_parameter("x_rot", -rot_y)
	#card_texture.material.set_shader_parameter("y_rot", -rot_x)
	#card_texture.material.set_shader_parameter("x_rot", -rot_y)
	#card_texture.material.set_shader_parameter("y_rot", -rot_x)

func callFree() -> void:
	self.mouse_filter = Control.MOUSE_FILTER_IGNORE
	if tween_destroy == null:
		tween_destroy = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
		tween_destroy.tween_property(card_texture.material, "shader_parameter/percentage", 0.0, 2.0).from(1.0)
		tween_destroy.parallel().tween_property(shadow, "self_modulate:a", 0.0, 1.4)
		tween_destroy.parallel().tween_property(statusview, "modulate:a", 0.0, 1.4)
	await tween_destroy.finished
	get_parent().queue_free()
	
	
	
	
func callInstantiate() -> void:
	if tween_spawn and tween_spawn.is_running():
		tween_spawn.kill()
	tween_spawn = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
	
	tween_spawn.tween_property(card_texture.material, "shader_parameter/percentage", 0.0, 0.0)
	tween_spawn.tween_property(shadow, "self_modulate:a", 1.0, 1.0).from(0.0)
	tween_spawn.tween_property(card_texture.material, "shader_parameter/percentage", 1.0, randf_range(0.2,2.0)).from(0.0)
	
	var m = card_texture.material.get("shader_parameter/burn_texture")

	m.get("noise").set("seed",randi_range(1, 99));
	
func blinded() -> void:
	var cardMaterial = card_texture.material;
	
	
	cardMaterial.set("shader_parameter/pulse_color", Color(0, 0, 0))
	cardMaterial.set("shader_parameter/progress", 1.0)

	
func callShine() -> void:
	var cardMaterial = card_texture.material;
	DampedOscillator.animate(card_texture.get_parent().get_parent(), "scale", 200.0, 10.0, 15.0, 0.75)
	var s = cardHitAnimation.instantiate()
	s.position = Vector2(0,0)
	card_texture.get_child(0).get_child(0).add_child(s)
	if tween_flash and tween_flash.is_running():
		tween_flash.kill()
	
	AudioManager.create_audio("CARD_HIT")

	
	tween_flash = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_CUBIC).set_parallel(true)
	tween_flash.tween_property(cardMaterial, "shader_parameter/progress", 0.0, 1.0).from(1.5)
#	tween_flash.tween_property(cardMaterial, "shader_parameter/x_rot", 0.0, 3.0).from(58.0)
	
func destroy() -> void:
	card_texture.use_parent_material = true
	if tween_destroy and tween_destroy.is_running():
		tween_destroy.kill()
	tween_destroy = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
	tween_destroy.tween_property(material, "shader_parameter/dissolve_value", 0.0, 2.0).from(1.0)
	tween_destroy.parallel().tween_property(shadow, "self_modulate:a", 0.0, 1.0)

func rotate_velocity(delta: float) -> void:
	if not following_mouse: return
	var center_pos: Vector2 = global_position - (size/2.0)
#	print("Pos: ", center_pos)
#	print("Pos: ", last_pos)
	# Compute the velocity
	velocity = (position - last_pos) / delta
	last_pos = self.position
	
#	print("Velocity: ", velocity)
	oscillator_velocity += velocity.normalized().x * velocity_multiplier
	
	# Oscillator stuff
	var force = -spring * displacement - damp * oscillator_velocity
	oscillator_velocity += force * delta
	displacement += oscillator_velocity * delta
	
	rotation = displacement

func handle_shadow(delta: float) -> void:
	# Y position is enver changed.
	# Only x changes depending on how far we are from the center of the screen
	var center: Vector2 = Vector2(750,324)
	#print(get_viewport_rect().size)
	var distance: float = global_position.x - center.x
	
	shadow.position.x = lerp(0.0, -sign(distance) * max_offset_shadow, abs(distance/(center.x))) - min_offset_shadow
#	print(shadow.position.x)

func follow_mouse(delta: float) -> void:
	if not following_mouse: return
	var mouse_pos: Vector2 = get_global_mouse_position()
	#global_position = mouse_pos - (size/2.0)
	

	#324 250 0 375
	var offsetY : float = main.position.y + parentHand.position.y + node2D.position.y + size.y - pivot_offset.y;
	var offsetX : float = main.position.x + parentHand.position.x + node2D.position.x + size.x - pivot_offset.x;
	
	position = mouse_pos - Vector2(offsetX,offsetY)
	
func handle_mouse_click(event: InputEvent) -> void:
	if not event is InputEventMouseButton: return
	if event.button_index != MOUSE_BUTTON_LEFT: return
	
	if not grabbable:
		return
		
	if event.is_pressed():
		following_mouse = true
		
		
	#else:
		# drop card
		#_on_drop_card()
		
func _on_drop_card(snap: bool) -> void:
	if tween_return and tween_return.is_running():
		tween_return.kill()
	if(snap):
		tween_return = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
		tween_return.tween_property(self, "position", ogOrigin, 0.3)
		tween_return.tween_property(node2D, "z_index", 0, 0)
	following_mouse = false
	#	collision_shape.set_deferred("disabled", false)
	if tween_handle and tween_handle.is_running():
		tween_handle.kill()
	if(snap):
		tween_handle = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
		tween_handle.tween_property(self, "rotation", 0.0, 0.3)
		
		
func _on_gui_input(event: InputEvent) -> void:
	
	if event is InputEventMouseButton and event.pressed:
		match event.button_index:
				MOUSE_BUTTON_RIGHT:
					cardview.DescriptionTextNext()
					popupContainer.UpdatePopup(cardview)
	handle_mouse_click(event)
	
	# Don't compute rotation when moving the card
	if following_mouse: return
	if not event is InputEventMouseMotion: return
	
	# Handles rotation
	# Get local mouse pos
	var mouse_pos: Vector2 = get_local_mouse_position()
	#print("Mouse: ", mouse_pos)
	#print("Card: ", position + size)
	var diff: Vector2 = (position + size) - mouse_pos

	var lerp_val_x: float = remap(mouse_pos.x, 0.0, size.x, 0, 1)
	var lerp_val_y: float = remap(mouse_pos.y, 0.0, size.y, 0, 1)
	#print("Lerp val x: ", lerp_val_x)
	#print("lerp val y: ", lerp_val_y)

	var rot_x: float = rad_to_deg(lerp_angle(-angle_x_max, angle_x_max, lerp_val_x))
	var rot_y: float = rad_to_deg(lerp_angle(angle_y_max, -angle_y_max, lerp_val_y))
	#print("Rot x: ", rot_x)
	#print("Rot y: ", rot_y)
	
	card_texture.material.set_shader_parameter("x_rot", -rot_y)
	card_texture.material.set_shader_parameter("y_rot", -rot_x)
#	shadow.material.set_shader_parameter("x_rot", -rot_y)
#	shadow.material.set_shader_parameter("y_rot", -rot_x)







func _on_mouse_entered() -> void:
#	node2D.top_level = true
	#print("Enter")
	#card_texture.z_index = 2
	#shadow.z_index = 2
	#PopupConstruct.Popup(Rect2i(Vector2i(global_position), Vector2i(size)),null)
	
	popupContainer.visible = true;
	popupContainer.PopupCheck(cardview)
	if node2D.z_index != 4:
		node2D.z_index = 2
	if tween_hover and tween_hover.is_running():
		tween_hover.kill()
	tween_hover = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_BACK)
	tween_hover.tween_property(self, "scale", Vector2(0.8, 0.8), 0.5)

	
func _on_mouse_exited() -> void:
	# Reset rotation
#	node2D.top_level = false
	#card_texture.z_index = 0
	#shadow.z_index = 0
	popupContainer.visible = false;
	popupContainer.DeleteChildren()
	
	if node2D.z_index != 4:
		node2D.z_index = 0
		
	if tween_rot and tween_rot.is_running():
		tween_rot.kill()
	tween_rot = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_CIRC).set_parallel(true)
	tween_rot.tween_property(card_texture.material, "shader_parameter/x_rot", 0.0, 0.5)
	tween_rot.tween_property(card_texture.material, "shader_parameter/y_rot", 0.0, 0.5)
	
	# Reset scale
	if tween_hover and tween_hover.is_running():
		tween_hover.kill()
	tween_hover = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_CUBIC)
	tween_hover.tween_property(self, "scale", Vector2(0.5,0.5), 0.25)


func _on_button_down():
	AudioManager.create_audio("CARD_DRAW")
	outline.visible = true
	node2D.z_index = 4
	var parentNode = get_parent().get_parent()
	parentNode.move_child(get_parent(), 0)
	parentNode.get_parent().move_child(parentNode, 1)
	


func _on_button_up():
	outline.visible = false
	node2D.z_index = 2
#	node2D.z_index = 0
