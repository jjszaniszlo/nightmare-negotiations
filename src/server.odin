package main

import "core:fmt"
import "core:net"
import "core:thread"

PORT :: 4556
THREADS :: 30

Server :: struct {
	socket : net.TCP_Socket,
	thread_pool : thread.Pool,
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

	return true
}

server_run :: proc(server: ^Server) {
	thread.pool_start(&server.thread_pool)

	for {
		
	}
}

@(private="file")
_server_accept_connection :: proc() {

}

server_destroy :: proc(server : ^Server) {
	thread.pool_destroy(&server.thread_pool)
}
