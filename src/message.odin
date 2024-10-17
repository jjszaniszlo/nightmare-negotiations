package main

MessageType :: enum(int) {
	UserAuthentication,
}

Message :: struct {
	id : MessageType,
	data : string,
}
