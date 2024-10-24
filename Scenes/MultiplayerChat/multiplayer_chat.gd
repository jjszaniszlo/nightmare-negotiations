extends Node2D

@onready var host = $Host
@onready var join = $Join
@onready var username = $Username
@onready var send = $Send
@onready var message = $Message
@onready var messages = $Messages

var usernm : String #Username
var msg : String #Msg 


func _on_host_pressed() -> void:
	var peer = ENetMultiplayerPeer.new() 
	peer.create_server(1027) #Make new server, currently has port 1027
	get_tree().set_multiplayer(SceneMultiplayer.new(),self.get_path())
	multiplayer.multiplayer_peer = peer
	joined()
	


func _on_join_pressed() -> void:
	var peer = ENetMultiplayerPeer.new()
	peer.create_client("127.0.0.1",1027) #Reference a different IP when forwarding to a server 
	get_tree().set_multiplayer(SceneMultiplayer.new(),self.get_path())
	multiplayer.multiplayer_peer = peer
	joined()

func _on_send_pressed() -> void:
	rpc("msg_rpc", usernm, message.text)
	message.text = "" #Clear text after every message, reset to blank string

@rpc ("any_peer","call_local")
@warning_ignore("shadowed_variable")
func msg_rpc(senders_usernm, data):
	messages.text += str(senders_usernm, ": ", data, "\n")
	messages.scroll_vertical = INF

func joined():
	host.hide()
	join.hide()
	username.hide()
	usernm = username.text 
