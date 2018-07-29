using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HttpNetwork : MonoBehaviour {

	public string URL = "www.google.com";
	public string responseTest;


	// Use this for initialization
	void Start () {
		WWW request = new WWW (URL);
		// Debug.Log(request.text);
		StartCoroutine(OnResponse(request));
	}
	

	private IEnumerator OnResponse(WWW req) {
		yield return req;

		responseTest = req.text;
	}

	// Update is called once per frame
	void Update () {
		Debug.Log(responseTest);
	}
}
