package main

import "core:fmt"
import "core:net"
import "core:sys/linux"
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
	peers : map[i32]Peer,
	fds : [dynamic]linux.Poll_Fd
}

server_init :: proc(server : ^Server) -> (ok: bool) {
	socket_fd, err := linux.socket(.INET6, .STREAM, { .NONBLOCK }, .TCP)
	if err != .NONE {
		fmt.printfln("create socket error %v", err)
		return
	}
	server.socket = cast(net.TCP_Socket)socket_fd
	//socket, listen_error := net.listen_tcp(net.Endpoint{
	//	address = net.IP4_Loopback,
	//	port = PORT,
	//})
	//if listen_error != nil {
	//	fmt.printfln("error listening on localhost:%d", PORT)
	//	return
	//}
	//
	//if err := net.set_blocking(socket, false); err != nil {
	//	fmt.printfln("err setting the server socket to non blocking: %v", err)
	//	return
	//}
	//server.socket = socket
	//fmt.printfln("listening on localhost:%d", PORT)

	alloc_err : mem.Allocator_Error
	server.peers, alloc_err = make(map[i32]Peer)
	if alloc_err != .None {	
		fmt.printfln("error allocating peers map %v", alloc_err)
		return
	}
	server.fds, alloc_err = make([dynamic]linux.Poll_Fd)
	if alloc_err != .None {	
		fmt.printfln("error allocating peers fds %v", alloc_err)
		return
	}

	_, alloc_err = append(&server.fds, linux.Poll_Fd{
		fd = linux.Fd(server.socket),
		events = { .IN },
	})
	if alloc_err != .None {	
		fmt.printfln("error appending server fd %v", alloc_err)
		return
	}
	return true
}

server_destroy :: proc(server : ^Server) {
	delete(server.peers)
}

server_run :: proc(server: ^Server) {
	for {
		_, err := linux.poll(server.fds[:], -1)
		if err != .NONE {
			fmt.printfln("polling failed: %v", err)
			break
		}

		_server_accept_incoming_connections(server)
	}
}

@(private="file")
_server_accept_incoming_connections :: proc(server: ^Server) -> (ok: bool) {
	for {
		peer_socket, peer_endpoint, peer_accept_err := net.accept_tcp(server.socket)
		if peer_accept_err != nil {
			if peer_accept_err.(net.Accept_Error) != .Would_Block {
				fmt.printfln("unexpected error: %v", peer_accept_err.(net.Accept_Error))
				return false
			}
			// all incoming connections handled
			return true
		}
		fmt.printfln("accepted peer endpoint: %v, sock: %v", peer_endpoint, peer_socket)

		// create random unique peer id
		peer_id := rand.int31()
		for (peer_id in server.peers) do peer_id = rand.int31()

		// add peer to peer map
		server.peers[peer_id] = Peer{
			id = peer_id,
			socket = peer_socket,
		}

		// append incoming peer file descriptor to server fds array
		_, err := append(&server.fds, linux.Poll_Fd{
			fd = linux.Fd(peer_socket),
			events = { .IN },
		})
		if err != .None {	
			fmt.printfln("error appending incoming peer fd %v", err)
		}
	}
	return false
}

//@(private="file")
//_server_accept_connection :: proc(server: ^Server) -> (ok:bool) {
//	// accept incoming socket
//	peer_socket, peer_endpoint, peer_accept_err := net.accept_tcp(server.socket)
//	if peer_accept_err != nil && peer_accept_err.(net.Accept_Error) != .Would_Block {
//		fmt.printfln("failed to accept tcp client! %v", peer_accept_err)
//		return
//	} else if peer_accept_err != nil && peer_accept_err.(net.Accept_Error) == .Would_Block {
//		// ignore Would_Block error
//		return
//	}
//
//	fmt.printfln("accepted client %v, %v", peer_endpoint, peer_socket)
//
//	// create random unique peer id
//	peer_id := rand.int31()
//	for (peer_id in server.peers) do peer_id = rand.int31()
//
//	// add peer to peer map
//	server.peers[peer_id] = Peer{
//		id = peer_id,
//		socket = peer_socket,
//	}
//
//	return true
//}
