package main

import "core:sys/linux"
import "core:net"

// manages each peer connection
Peer :: struct {
	id : i32,
	socket : net.TCP_Socket,
}
