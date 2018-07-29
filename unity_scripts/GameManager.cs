using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public enum GameState
    {
        WAIT_TO_START,
        PLAYING,
        PAUSED,
        FINISHED
    }


    public static GameState gameState;

	public static CharacterController character;

	// Difficulty
	public int difficulty;
	public bool randomPosition;
	public float randomPositionRange;

	public GameObject[] barriers; 
	public Transform spwanPointGroup;
	Transform[] spwanPoints;
	float totalDistance;
	Vector3 prePosition;
	public static OVRScreenFade screenFade;
	private static Vector3 initialPosition;
    private static Quaternion initialOrientation;


	// Use this for initialization
	void Start () {
		if (character == null) {
			character = FindObjectOfType<CharacterController>();
		}
		prePosition = character.transform.position;
		
		
		gameState = GameState.WAIT_TO_START;
        if (character != null)
        {
            initialPosition = character.transform.position;
            initialOrientation = character.transform.rotation;
			screenFade = character.GetComponentInChildren<OVRScreenFade>();
            Debug.Log(initialPosition);
        }
		

		// Spwan barriers
		spwanPoints = new Transform[spwanPointGroup.childCount];
		for(int i = 0; i < spwanPointGroup.childCount; i++) {
			// spwanPoints[i] = spwanPointGroup.GetChild(i);
			int difficultyRandomNum = Random.Range(0, 100);
			
			Vector3 spwanPosition = spwanPointGroup.GetChild(i).position;


			var spwanRotation = spwanPointGroup.GetChild(i).rotation;
			if (randomPosition) {
				spwanPosition.x = Random.Range(spwanPosition.x - randomPositionRange, spwanPosition.x + randomPositionRange);
				spwanPosition.z = Random.Range(spwanPosition.z - randomPositionRange, spwanPosition.z + randomPositionRange);
				spwanRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
			}

			

			if (difficultyRandomNum < difficulty) {
				Instantiate(barriers[Random.Range(0, barriers.Length)], spwanPosition, spwanRotation);
			}
		}



	}
	
	// Update is called once per frame
	void Update () {
		if (gameState == GameState.FINISHED)
        {
            // Reset the players position
            ResetGame();
        }


		// Calculate the total distance
		// totalDistance += Vector3.Distance(character.transform.position, prePosition);
		// prePosition = character.transform.position;
	}


	public static void SwitchState(string reply)
    {
        if (reply == "start")
        {
            if (gameState == GameState.WAIT_TO_START)
            {
                StartGame();
            }
            else
            {
                ResetGame();
                StartGame();
            }
            gameState = GameState.PLAYING;
        }
        else if (reply == "stop" && gameState == GameState.PLAYING)
        {
            gameState = GameState.FINISHED;
        }
        else if (reply == "pause" && gameState == GameState.PLAYING)
        {
            gameState = GameState.PAUSED;
        }
        else if (reply == "continue" && gameState == GameState.PAUSED)
        {
            gameState = GameState.PLAYING;
        }
        else if (reply == "received" && gameState == GameState.FINISHED)
        {
            gameState = GameState.WAIT_TO_START;
        }
        else
        {
            // Unknown
            Debug.Log("Unknown command");
        }
    }

    public static void StartGame()
    {
        FindObjectOfType<WebsocketUploadData>().sendSummary = false;
		screenFade.FadeIn();
    }

    public static void ResetGame()
    {
		screenFade.SetBlack();
        screenFade.FadeOut();
        character.transform.position = initialPosition;
        character.transform.rotation = initialOrientation;
    }
}
