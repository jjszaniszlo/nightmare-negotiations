package main

import "core:sync"
import "core:fmt"
import "core:net"
import "core:mem/virtual"
import "core:mem"

// manages each peer connection
Peer :: struct {
	id : i32,
	socket : net.TCP_Socket,
	arena : virtual.Arena,
	mutex : sync.Mutex,
}

peer_init :: proc(peer: ^Peer, id : i32, socket : net.TCP_Socket) -> (ok: bool) {
	arena_alloc_err := virtual.arena_init_growing(&peer.arena, 1 * mem.Kilobyte)
	if arena_alloc_err != .None {
		fmt.printfln("failed to create peer arena! %v", arena_alloc_err)
		return
	}
	peer.id = id
	peer.socket = socket
	return true
}
