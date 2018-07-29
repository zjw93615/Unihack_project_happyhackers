# -*- coding:utf-8 -*-
#

import socket
import threading
import socketserver
import json, types,string
import os, time

class ThreadedTCPRequestHandler(socketserver.BaseRequestHandler):
    def handle(self):
        data = self.request.recv(1024)
        jdata = json.loads(data.decode('utf-8'))
        print("Receive data from '%r'"% (data))
        print("Receive jdata from '%r'"% (jdata))
        rec_posX = jdata[0]['posX']
        rec_posY = jdata[0]['posY']

        cur_thread = threading.current_thread()
        response = [{'thread':cur_thread.name,'posX':rec_posX,'posY':rec_posY}]

        jresp = json.dumps(response)
        self.request.sendall(jresp.encode('utf-8'))
        #rec_cmd = "proccess "+rec_posX+" -o "+rec_posY
        #print("CMD '%r'" % (rec_cmd))
        #os.system(rec_cmd)

class ThreadedTCPServer(socketserver.ThreadingMixIn, socketserver.TCPServer):
    pass

if __name__ == "__main__":
    # Port 0 means to select an arbitrary unused port
    HOST, PORT = "localhost", 50001

    socketserver.TCPServer.allow_reuse_address = True
    server = ThreadedTCPServer((HOST, PORT), ThreadedTCPRequestHandler)
    ip, port = server.server_address

    # Start a thread with the server -- that thread will then start one
    # more thread for each request
    server_thread = threading.Thread(target=server.serve_forever)
    # Exit the server thread when the main thread terminates
    server_thread.daemon = True
    server_thread.start()
    print("Server loop running in thread:", server_thread.name)
    print(" .... waiting for connection")

    # Activate the server; this will keep running until you
    # interrupt the program with Ctrl-C
    server.serve_forever()
