package main

import "core:net"

// manages each peer connection
Peer :: struct {
	id : int,
	socket : net.TCP_Socket,
}
