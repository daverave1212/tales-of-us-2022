extends Sprite

var is_being_dragged = false

func _input(event):
	if U.is_mouse_left_clicked(event):
		is_being_dragged = true
	elif U.is_mouse_left_released(event):
		is_being_dragged = false

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.



func _process(delta_time):
	# var mouse_position = get_viewport().get_mouse_position()
	var mouse_position = get_global_mouse_position()
	if is_being_dragged:
		global_position = mouse_position
		is_overlapping_card_slot()
	
	


func _on_Area2D_area_entered(area):
	pass




# Utility functions
func is_overlapping_card_slot():
	var area2D = $Area2D
	var overlapping_areas = area2D.get_overlapping_areas()
	
	
	
	
