using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

	public float speed;
	public float stepWide;
	public float stepHight;
	public float stepHightFacter;
	public float gravity = 20.0F;
	public LayerMask raycastLayer;
	public int stepDetectThreshold;

	public GameObject wayPointGroup;

	public Transform arrowUI;

	public bool canMoveHigher;
	Transform[] wayPoints;
	float halfStepWide;
	float stepX;
	float stepY;
	CharacterController character;
	Vector3 inputDirection;
	public bool stepUp;
	int positionY;
	int prePositionY;
	int stepUpdateCount;
	int moveUpBufferData;
	int moveDownBufferData;
	float stepTime;
	int targetWaypointIndex;
	Transform targetWaypoint;
	float highestHight;
	WebsocketUploadData webSocket;




	// Use this for initialization
	void Start () {
		character = GetComponent<CharacterController>();
		
		halfStepWide = stepWide / 2;
		stepX = 0;
		stepY = 0;
	 	stepTime = 0;
		canMoveHigher = false;
		webSocket = FindObjectOfType<WebsocketUploadData>();


		wayPoints = new Transform[wayPointGroup.transform.childCount];
		for(int i = 0; i < wayPointGroup.transform.childCount; i++) {
			// spwanPoints[i] = spwanPointGroup.GetChild(i);
			wayPoints[i] = wayPointGroup.transform.GetChild(i);
		}

		targetWaypointIndex = 0;
		targetWaypoint = wayPoints[targetWaypointIndex];


	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		if(Physics.Raycast(transform.position, -transform.up, out hit)) {
			

			if(canMoveHigher) {
				highestHight = Mathf.Max(stepHight, highestHight);
				stepY = hit.point.y + character.height * 1.1f + highestHight / stepHightFacter;
			}else {
				stepY = hit.point.y + character.height * 1.1f;
			}
			
			Vector3 position = transform.position;
			position.y = stepY;
			transform.position = position;

			float moveDistance = 0f;
			if(stepUp) {
				moveDistance = moveUpBufferData;
			}else {
				moveDistance = moveDownBufferData;
			}
			moveUpBufferData = 0;
			moveDownBufferData = 0;
			stepTime += Time.deltaTime;
			Debug.Log(moveDistance);

			// Test only
			if(moveDistance == 0) {
				moveDistance = Input.GetAxis("Vertical");
			}

			inputDirection = new Vector3(Input.GetAxis("Horizontal"), 0, moveDistance);
			inputDirection = transform.TransformDirection(inputDirection);
			inputDirection *= speed;

			// inputDirection.y -= gravity * Time.deltaTime;
			character.Move(inputDirection * Time.deltaTime);

			if(targetWaypointIndex <= wayPoints.Length - 2)
			{
				if(Vector3.Distance(this.transform.position, targetWaypoint.position) < 3f)
				{
					targetWaypointIndex++;
					targetWaypoint = wayPoints[targetWaypointIndex];
				}
			}

			Vector3 lookPos = targetWaypoint.position - this.transform.position;
			lookPos.y = 0;
			var q1 = Quaternion.LookRotation(lookPos);
			if(moveDistance > 0) {
            	this.transform.rotation = Quaternion.Slerp(this.transform.rotation, q1, Time.deltaTime);

			} 

			// arrowUI.rotation = Quaternion.Slerp(this.transform.rotation, q1, Time.deltaTime * 3);
 

		}

		// character.Move(transform.forward * speed * Time.deltaTime);
		
		
	}

	public void SetData(int x, int y) {
		
		webSocket.SendData(x, y);
		if(Mathf.Abs(prePositionY - y) < 2)
			return;

		prePositionY = positionY;
		positionY = y;

		// Not the first packet
		if(prePositionY != 0) {
			if(prePositionY > positionY) {

				moveDownBufferData += prePositionY - positionY;
				// Conform is moving down
				
				moveUpBufferData = 0;
				// First Time call move down, Send step hight
				if(stepUp) {
					// Send step hight
					webSocket.SendData(x, y, stepHight, stepTime);
					// Send step time

					// stepTime = 0;
					// stepHight = 0;
				}
				stepUp = false;
			}else {
				moveUpBufferData += positionY - prePositionY;
			
				// First move up
				if (!stepUp) {
					// Reset step time
					stepTime = 0;
					stepHight = 0;
				}

				moveDownBufferData = 0;
				stepUp = true;
				stepHight += moveUpBufferData;

				
			}
		}
	}


	/// <summary>
	/// OnTriggerStay is called once per frame for every Collider other
	/// that is touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerEnter(Collider other)
	{
		canMoveHigher = true;
		highestHight = 0;
	}

	/// <summary>
	/// OnTriggerExit is called when the Collider other has stopped touching the trigger.
	/// </summary>
	/// <param name="other">The other Collider involved in this collision.</param>
	void OnTriggerExit(Collider other)
	{
		canMoveHigher = false;
		highestHight = 0;
	}
}
