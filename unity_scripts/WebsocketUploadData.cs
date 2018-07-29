using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[Serializable]
public class SingleData
{
    public long timestamp;
    public float posX;
    public float posY;
}

[Serializable]
public class RealtimeData
{
    public long timestamp;
    public List<SingleData> data;
}

[Serializable]
public class SummarizedData
{
    public long timestamp;

    // steps walked
    public int totalSteps;
    public List<float> stepTimes;
    public List<float> stepHeights;
    public float rate;
    //public float balance;

    public SummarizedData()
    {
        totalSteps = 0;
        stepTimes = new List<float>();
        stepHeights = new List<float>();
        rate = 0;
    }
}

public class WebsocketUploadData : MonoBehaviour
{
	public string ip = "localhost";
	public string port = "8000";

	private int x = 0;
	private float currentTime;
	private float TIME_INTERVAL = 1.0f;

    public SingleData data;
    public SummarizedData summary;

    public WebSocket webSocket;

    public bool sendSummary = false;

    private string buffer = "";

    void Start()
    {
        StartCoroutine("WebSocketConnection");
    }

    IEnumerator WebSocketConnection()
	{
        currentTime = TIME_INTERVAL;
        summary = new SummarizedData();

        WebSocket w = new WebSocket (new Uri ("ws://" + ip + ":" + port));
        webSocket = w;
		yield return StartCoroutine (w.Connect ());
		Debug.Log("Start");

        while (true) {
			yield return new WaitForEndOfFrame();

            // Send data to the server regularly
            if (currentTime <= 0) {
                // data = GetData();
                // char[] textToSend = new char[buffer.Length];
                // buffer.CopyTo(0, textToSend, 0, buffer.Length);
				// SendData(textToSend.ToString());
                SendData(buffer);
                buffer = ""; // clear buffer
				currentTime = TIME_INTERVAL;
			} else {
				currentTime -= Time.deltaTime;
			}

			string reply = w.RecvString ();
			if (reply != null) {
				Debug.Log (reply);
				HandleCommand(reply);
			}
			
			if (w.error != null) {
				Debug.LogError ("Error: " + w.error);
				// break;
			}

			if (GameManager.gameState == GameManager.GameState.FINISHED) {
                // Upload the summarized data, make sure it only send once
                if (!sendSummary)
                {
                    SendData(w, summary);
                    sendSummary = true;
                }
                // break;
            }

            yield return 0;
		}
		w.Close ();
	}

    public long UnixTimeNow()
    {
        var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
        return (long)timeSpan.TotalMilliseconds;
    }

    void SendData(WebSocket w, SingleData data)
    {
        // Send JSON string
        w.SendString(JsonConvert.SerializeObject(data));
    }

    void SendData(WebSocket w, RealtimeData data) {
		// Send JSON string
        w.SendString(JsonConvert.SerializeObject(data));
	}

    void SendData(WebSocket w, SummarizedData data)
    {
        data.timestamp = UnixTimeNow();
        // Send JSON string
        w.SendString(JsonConvert.SerializeObject(data));
    }

    public void SendData(String data)
    {
        Debug.Log("SendDataString:" + data);
        if (data.Length == 0) return;
        // Send JSON string
        foreach (string str in data.Split("\n".ToCharArray())) {
            webSocket.SendString(str);
        }
    }

    public void SendData(int posX = -1, int posY = -1, float stepHeight = -1, float stepTime = -1)
    {
        string outputStr = "[";
        bool ishead = true;

        if (posX > 0 && posY > 0)
        {
            outputStr += String.Format("{{\"posX\":{0},\"posY\":{1}}}", posX, posY);
            ishead = false;
        }
        if (stepHeight > 0)
        {
            if (!ishead)
            {
                outputStr += ",";
                ishead = false;
            }
            outputStr += String.Format("{{\"stepHeight\":{0}}}", stepHeight);
            summary.stepHeights.Add(stepHeight);
        }
        if (stepTime > 0)
        {
            if (!ishead)
            {
                outputStr += ",";
                ishead = false;
            }
            outputStr += String.Format("{{\"stepTime\":{0}}}", stepTime);
            summary.stepTimes.Add(stepTime);
        }

        outputStr += "]";

        // webSocket.SendString(outputStr);
        if(buffer.Length > 0) {
            buffer += "\n" + outputStr;
        }
        else {
            buffer = outputStr;
        }
    }

    void HandleCommand(String reply) {
        GameManager.SwitchState(reply);
	}
}

