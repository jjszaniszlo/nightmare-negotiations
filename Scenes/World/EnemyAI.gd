extends CharacterBody3D

var patrol_speed = 5.0
var chase_speed = 7.0

var gravity = -9.8
var Velocity = Vector3.ZERO
var jump_force = 20.0

var waypoints = []
var current_waypoint = 0

var player = null
enum State {
	PATROLLING,
	CHASING,
	ATTACKING
}
var state = State.PATROLLING

func _ready():
	# Gather all the waypoints (Marker3D) for patrol routes
	for node in get_parent().get_children():
		if node is Marker3D:
			waypoints.append(node)

func _physics_process(delta):
	# Apply gravity
	if not is_on_floor():
		Velocity.y += gravity * delta
	else:
		Velocity.y = 0  # Reset vertical Velocity when on the floor

	if state == State.PATROLLING:
		patrol(delta)
		check_player_detection()
	elif state == State.CHASING:
		chase_player(delta)

	move_and_slide()

func patrol(delta):
	if waypoints.size() == 0:
		return  
	var target_pos = waypoints[current_waypoint].global_transform.origin
	var direction = (target_pos - global_transform.origin).normalized()
	Velocity.x = direction.x * patrol_speed * delta
	Velocity.z = direction.z * patrol_speed * delta

	# Move to the next waypoint if close enough
	if global_transform.origin.distance_to(target_pos) < 1.0:
		current_waypoint = (current_waypoint + 1) % waypoints.size()

func check_player_detection():
	if $RayCast3D.is_colliding():
		var collider = $RayCast3D.get_collider()
		if collider.name == "CharacterBody3D":
			player = collider
			state = State.CHASING

func chase_player(delta):
	if player == null:
		return  # No target player to chase

	var player_pos = player.global_transform.origin
	var direction = (player_pos - global_transform.origin).normalized()

	Velocity.x = direction.x * chase_speed * delta
	Velocity.z = direction.z * chase_speed * delta

	if global_transform.origin.distance_to(player_pos) > 20.0:
		state = State.PATROLLING
