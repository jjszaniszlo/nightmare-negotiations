package main

import "core:net"
import "core:os"
import "core:thread"

main :: proc() {
	server : Server
	if ok := server_init(&server); !ok do os.exit(1)
	server_run(&server)
	server_destroy(&server)
}
