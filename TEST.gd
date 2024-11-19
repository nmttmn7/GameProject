@tool
extends Node

@export_category("Stuff")

@export var use_custom_stuff: bool: 
	set(value):
		use_custom_stuff = value
		property_list_changed.emit()
@export var custom_variable: String
