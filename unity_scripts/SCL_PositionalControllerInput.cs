using UnityEngine;
using System;
using System.Threading;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

public class SCL_PositionalControllerInput : MonoBehaviour, SCL_IClientSocketHandlerDelegate {

	public int portNumber;
	public string ipAddress;
	public int maxClients;
	public CharacterMovement character;
	private readonly object positionLock = new object();
	private Vector3 position = new Vector3();

	private SCL_SocketServer socketServer;

	// Use this for initialization
	void Start () {
		Debug.Log("Hello From Me");

		SCL_IClientSocketHandlerDelegate clientSocketHandlerDelegate = this;
		if (maxClients == 0)
			maxClients = 10;
		string separatorString = "&";
		if(portNumber <= 1024)
			portNumber = 8888;

		if(ipAddress.Equals("")) {
			ipAddress = "127.0.0.1";
		}
		Encoding encoding = Encoding.UTF8;

		this.socketServer = new SCL_SocketServer(
			clientSocketHandlerDelegate, maxClients, separatorString, ipAddress, portNumber, encoding);

		this.socketServer.StartListeningForConnections();

		Debug.Log (String.Format (
			"Started socket server at {0} on port {1}", 
			this.socketServer.LocalEndPoint.Address, this.socketServer.PortNumber));
	}

	void OnApplicationQuit() {
		Debug.Log ("Cleaning up socket server");
		this.socketServer.Cleanup();
		this.socketServer = null;
	}

	// this delegate method will be called on another thread, so use locks for synchronization
	public void ClientSocketHandlerDidReadMessage(SCL_ClientSocketHandler handler, string message)
	{
		

		// message = message.Trim(new char[] {'[', ']'});


		// int startIndex = message.LastIndexOf('{');
		// int endIndex = message.LastIndexOf('}');
		// message = message.Substring(startIndex, endIndex + 1);

		// int y = 0;
		// int x = 0;
		// string[] strs = message.Split(',');
		// for(int i = 0; i < strs.Length; i++) {
		// 	string temp = Regex.Match(strs[i], @"\d+").Value;
		// 	if(strs[i].Contains("X")) {
		// 		x = Int32.Parse(temp);
		// 	}else {
		// 		y = Int32.Parse(temp);
		// 	}
		// }

		// character.SetData (x, y);

		// Debug.Log(str);

		// StepData data = JsonUtility.FromJson<StepData>(message);

		Debug.Log(message);

		new Thread(() => 
		{
			Thread.CurrentThread.IsBackground = true; 
			int startIndex = message.LastIndexOf('{');
			int endIndex = message.LastIndexOf('}');
			message = message.Substring(startIndex, endIndex + 1);

			int y = 0;
			int x = 0;
			string[] strs = message.Split(',');
			for(int i = 0; i < strs.Length; i++) {
				string temp = Regex.Match(strs[i], @"\d+").Value;
				if(strs[i].Contains("X")) {
					x = Int32.Parse(temp);
				}else {
					y = Int32.Parse(temp);
				}
			}

			character.SetData (x, y);
		}).Start();


		// StartCoroutine(ProcessData(message));

		// Debug.Log(data.posY);

	}

	public static void ProcessData(System.Object obj) {


		string message = (string) obj;
		
	}

}

[Serializable]
public class StepData {
	public int posX;
	public int posY;
}
