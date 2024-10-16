package main

import "core:fmt"
import "core:net"
import "core:math/rand"
import "core:mem"
import "core:mem/virtual"
import "core:thread"
import "core:time"
import "core:sync"

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
	for {
		thread.pool_start(&server.thread_pool)
		defer thread.pool_finish(&server.thread_pool)

		_server_dispatch_disconnect_handlers(server)

		accept_ok := _server_accept_connection(server)
		if !accept_ok do continue
	}
}

@(private="file")
_server_dispatch_disconnect_handlers :: proc(server: ^Server) {
	for _, &peer in server.peers {	
		thread.pool_add_task(
			&server.thread_pool,
			virtual.arena_allocator(&peer.arena),
			_server_handle_peer_disconnect,
			&peer)
	}
}

@(private="file")
_server_handle_message :: proc(t: thread.Task) {
	peer := cast(^Peer)t.data

	sync.lock(&peer.mutex)
	defer sync.unlock(&peer.mutex)

	alloc := virtual.arena_allocator(&peer.arena)

	buf, alloc_err := make([]byte, 10, allocator = alloc)
	if alloc_err != .None {	
		fmt.printfln("failed to allocate buffer for %v", peer)
		net.close(peer.socket)
		return
	}
	defer delete(buf, allocator = alloc)

	_, err := net.recv_tcp(peer.socket, buf[:5])
	fmt.printfln("message: %v", buf)
}

@(private="file")
_server_handle_peer_disconnect :: proc(t: thread.Task) {
	peer := cast(^Peer)t.data

	sync.lock(&peer.mutex)
	defer sync.unlock(&peer.mutex)

	net.set_blocking(peer.socket, false)
	defer net.set_blocking(peer.socket, true)

	_, err := net.recv(peer.socket, nil)
	fmt.printfln("%v", err)
}

@(private="file")
_server_accept_connection :: proc(server: ^Server) -> (ok:bool) {
	// accept incoming socket
	peer_socket, peer_endpoint, peer_accept_err := net.accept_tcp(server.socket)
	if peer_accept_err != nil {
		fmt.printfln("failed to accept tcp client! %v", peer_accept_err)
		return
	}
	fmt.printfln("accepted client %v, %v", peer_endpoint, peer_socket)

	// create random unique peer id
	peer_id := rand.int31()
	for (peer_id in server.peers) do peer_id = rand.int31()

	// add peer to peer map
	server.peers[peer_id] = Peer{}
	peer := &server.peers[peer_id]
	peer_init(peer, peer_id, peer_socket)

	// dispatch thread from thread pool
	thread.pool_add_task(
		&server.thread_pool,
		virtual.arena_allocator(&peer.arena),
		_server_handle_message,
		peer)

	return true
}
