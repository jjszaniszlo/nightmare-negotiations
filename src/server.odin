package main

import "core:fmt"
import "core:net"
import "core:thread"

PORT :: 4556
THREADS :: 30

Server :: struct {
	socket : net.TCP_Socket,
	thread_pool : thread.Pool,
	peers : [dynamic]Peer,
}

server_init :: proc(server : ^Server) -> (ok: bool) {
	socket, listen_error := net.listen_tcp(net.Endpoint{
		address = net.IP4_Loopback,
		port = PORT,
	})

	if listen_error != nil {
		fmt.printfln("error listening on localhost:%d", PORT)
		return
	}
	server.socket = socket
	fmt.printfln("listening on localhost:%d", PORT)

	thread.pool_init(&server.thread_pool, context.allocator, THREADS)

	server.peers = make([dynamic]Peer)

	return true
}

server_destroy :: proc(server : ^Server) {
	thread.pool_destroy(&server.thread_pool)
	delete(server.peers)
}

server_run :: proc(server: ^Server) {
	thread.pool_start(&server.thread_pool)

	for {
		peer, peer_ok := _server_accept_connection(server)
		if !peer_ok do continue

		fmt.printfln("accepted client %v", peer)
	}
}

@(private="file")
_server_accept_connection :: proc(server: ^Server) -> (peer: Peer, ok:bool) {
	accept_socket, accept_endpoint, accept_err := net.accept_tcp(server.socket)
	if accept_err != nil {
		fmt.printfln("failed to accept tcp client!")
		return
	}

	peer.id = 0
	peer.socket = accept_socket

	_, err := append(&server.peers, peer)
	if err != .None {
		fmt.printfln("failed to add peer to peers array! %v\n", err)
		return
	}

	return peer, true
}
