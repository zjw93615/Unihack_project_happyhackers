
using UnityEngine;
using System.Collections;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Text;
using System;
public class TCPServer:MonoBehaviour{
    public bool isConnection;
    // Use this for initialization
    void Start(){
        isConnection=false;
        print ("StartCoroutine");
        StartCoroutine(Update1());
    }
   
    // Update is called once per frame
    IEnumerator Update1()
    {  
        TcpListener server=null;  
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 8888;
            IPAddress localAddr = IPAddress.Parse("192.168.43.93");
           
            server = new TcpListener(port);
            // server = new TcpListener(localAddr, port);
           
            // Start listening for client requests.
            server.Start();
           
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;
           
            // Enter the listening loop.
            while(!isConnection)
            {
                Debug.Log("Waiting for a connection... ");
               
                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();  
                if(client!=null){
 
                    Debug.Log("Connected!");
                    isConnection=true;
                    client.Close();
                    //break;
 
                }
                data = null;
               
                // Get a stream object for reading and writing
                NetworkStream stream = client.GetStream();
               
                int i;
               
                // Loop to receive all the data sent by the client.
                while((i = stream.Read(bytes, 0, bytes.Length))!=0)
                {  
                    // Translate data bytes to a ASCII string.
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    Debug.Log("Received:"+ data);
                   
                    // Process the data sent by the client.
                    data = data.ToUpper();
                   
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
                   
                    // Send back a response.
                    stream.Write(msg, 0, msg.Length);
                    Debug.Log("Sent:"+ data);          
                }
 
                // Shutdown and end connection
                client.Close();
            }
        }
        catch(SocketException e)
        {
            Debug.Log("SocketException:"+ e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }
 
        yield return null;
    }
}