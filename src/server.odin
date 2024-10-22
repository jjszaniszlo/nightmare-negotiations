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
import "core:encoding/json"
import "core:container/queue"

PORT :: 4556

Server :: struct {
	socket : net.TCP_Socket,
	peers : map[net.TCP_Socket]Peer,
	fds : [dynamic]linux.Poll_Fd,

	message_queue : queue.Queue(Message),
}

server_init :: proc(server : ^Server) -> (ok: bool) {
	socket, err := net.listen_tcp(net.Endpoint{
		address = net.IP4_Loopback,
		port = PORT,
	})
	if err != nil {
		fmt.printfln("error listiong on port %d: %v", PORT, err)
		return
	}
	server.socket = socket
	fmt.printfln("listening on localhost:%d", PORT)

	// set options
	net.set_option(socket, .Reuse_Address, true)
	net.set_blocking(socket, false)

	// allocate memory for data structures
	alloc_err : mem.Allocator_Error
	server.peers, alloc_err = make(map[net.TCP_Socket]Peer)
	if alloc_err != .None {	
		fmt.printfln("error allocating peers map %v", alloc_err)
		return
	}
	server.fds, alloc_err = make([dynamic]linux.Poll_Fd)
	if alloc_err != .None {	
		fmt.printfln("error allocating peers fds %v", alloc_err)
		return
	}

	// add socket file descriptor to fd list
	_, alloc_err = append(&server.fds, linux.Poll_Fd{
		fd = linux.Fd(socket),
		events = { .IN },
	})
	if alloc_err != .None {	
		fmt.printfln("error appending server fd %v", alloc_err)
		return
	}

	queue.init(&server.message_queue)

	return true
}

server_destroy :: proc(server : ^Server) {
	net.close(server.socket)
	delete(server.fds)
	delete(server.peers)
	queue.destroy(&server.message_queue)
}

server_run :: proc(server: ^Server) {
	for {
		// poll fd list
		_, err := linux.poll(server.fds[:], -1)
		if err != .NONE {
			fmt.printfln("polling failed: %v", err)
			break
		}

		// handle incoming connections
		_server_accept_incoming_connections(server)
		_server_receive_incoming_messages(server)

		_server_handle_messages(server)
	}
}

@(private="file")
_server_handle_messages :: proc(server : ^Server) {
	for queue.len(server.message_queue) > 0 {
		message := queue.pop_front(&server.message_queue)
		switch message.id {
		case .UserAuthentication:
			// do data base stuff
		}
	}
}

@(private="file")
_server_receive_incoming_messages :: proc(server : ^Server) -> (ok: bool) {	
	for &fd, i in server.fds[:] {
		if fd.revents == {} do continue;
		if fd.revents != { .IN } {
			fmt.printfln("unsupported message: %v", fd.revents)
			return
		}
		peer, ok := &server.peers[net.TCP_Socket(fd.fd)]
		if !ok do continue

		buf : [2048]byte
		bytes_read := 0

		for {
			n, err := net.recv_tcp(peer.socket, buf[bytes_read:])
			if err != nil {
				if err.(net.TCP_Recv_Error) == .Timeout {
					message : Message
					fmt.printfln("%s", buf[:bytes_read])
					json_err := json.unmarshal(buf[:bytes_read], &message)
					if json_err != nil {	
						fmt.printfln("json error: ", json_err)
						break
					}
					queue.push_back(&server.message_queue, message)
					break
				} else if err.(net.TCP_Recv_Error) == .Shutdown {
					fmt.printfln("closing peer: %v", peer)
					_server_cleanup_connection(server, peer, i)
					break
				} else {
					fmt.printfln("unexpected message error: %v", err.(net.TCP_Recv_Error))
				}
			} else {
				if n == 0 {
					fmt.printfln("closing peer: %v", peer)
					_server_cleanup_connection(server, peer, i)
					break
				}
			}
			bytes_read += n
		}
	}
	return true
}

@(private="file")
_server_cleanup_connection :: proc(server : ^Server, peer : ^Peer, fd_index : int) {
	net.close(peer.socket)
	delete_key(&server.peers, peer.socket)
	unordered_remove(&server.fds, fd_index)
}

@(private="file")
_server_accept_incoming_connections :: proc(server: ^Server) -> (ok: bool) {
	if server.fds[0].revents == {} do return;
	if server.fds[0].revents != { .IN } {
		fmt.printfln("unsupported event: %v", server.fds[0].revents)
		return
	}	
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
		net.set_blocking(peer_socket, false)

		// create random unique peer id
		peer_id := rand.int31()

		// add peer to peer map
		server.peers[peer_socket] = Peer{
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


@(private="file")
_unwrap_os_addr :: proc "contextless" (endpoint: net.Endpoint)->(linux.Sock_Addr_Any) {
	switch address in endpoint.address {
	case net.IP4_Address:
		return {
			ipv4 = {
				sin_family = .INET,
				sin_port = u16be(endpoint.port),
				sin_addr = ([4]u8)(endpoint.address.(net.IP4_Address)),
			},
		}
	case net.IP6_Address:
		return {
			ipv6 = {
				sin6_port = u16be(endpoint.port),
				sin6_addr = transmute([16]u8)endpoint.address.(net.IP6_Address),
				sin6_family = .INET6,
			},
		}
	case:
		unreachable()
	}
}
