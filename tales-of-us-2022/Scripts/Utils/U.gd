
class_name U

static func is_mouse_left_clicked(event):
	return (event is InputEventMouseButton and event.pressed and event.button_index == BUTTON_LEFT)
	
static func is_mouse_left_released(event):
	return (event is InputEventMouseButton and (not event.pressed) and event.button_index == BUTTON_LEFT)
