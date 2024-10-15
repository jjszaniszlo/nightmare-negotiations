package main

import "core:fmt"
import "core:net"
import "core:thread"
import "core:mem/virtual"
import "core:mem"
import "core:math/rand"
import "core:time"

PORT :: 4556
THREADS :: 30

Server :: struct {
	socket : net.TCP_Socket,
	thread_pool : thread.Pool,
	peers : map[i32]Peer,
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

	server.peers = make(map[i32]Peer)

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
	}
	thread.pool_finish(&server.thread_pool)
}

@(private="file")
_server_handle_message :: proc(t: thread.Task) {
	peer := cast(^Peer)t.data

	time.sleep(2e9)

	fmt.printfln("closing client socket: %v", peer.socket)
	net.close(peer.socket)
}

@(private="file")
_server_close_disconnected_sockets :: proc(server: ^Server) {
	
}

@(private="file")
_server_accept_connection :: proc(server: ^Server) -> (peer: Peer, ok:bool) {
	accept_socket, accept_endpoint, accept_err := net.accept_tcp(server.socket)
	if accept_err != nil {
		fmt.printfln("failed to accept tcp client! %v", accept_err)
		return
	}
	fmt.printfln("accepted client %v, %v", accept_endpoint, accept_socket)

	peer_arena: virtual.Arena
	arena_alloc_err := virtual.arena_init_growing(&peer_arena, 1 * mem.Kilobyte)
	if arena_alloc_err != nil {
		fmt.printfln("failed to create peer arena! %v", arena_alloc_err)

		net.close(accept_socket)
		return
	}
	client_alloc := virtual.arena_allocator(&peer_arena)

	// make sure that random id is unique (though chances are it is)
	random_id := rand.int31()
	for (random_id in server.peers) do random_id = rand.int31()

	peer.id = random_id
	peer.socket = accept_socket

	server.peers[peer.id] = peer

	thread.pool_add_task(
		&server.thread_pool,
		client_alloc,
		_server_handle_message,
		&server.peers[peer.id])

	return peer, true
}
