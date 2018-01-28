using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoBatMovement : MonoBehaviour {
	private Vector3 startingPosition;
	public int speed;
	private bool directionUp;
	private int distanceCounter = 0;

	// Use this for initialization
	void Start () {
		startingPosition = transform.position;
		directionUp = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (directionUp) {
			transform.position = Vector3.MoveTowards (
				transform.position, 
				new Vector3 (startingPosition.x, startingPosition.y + 10, startingPosition.z), 
				speed * Time.deltaTime);
			distanceCounter++;
		}
		else {
			transform.position = Vector3.MoveTowards (
				transform.position, 
				new Vector3 (startingPosition.x, startingPosition.y - 10, startingPosition.z), 
				speed * Time.deltaTime);
			distanceCounter++;
		}
		if (distanceCounter >= 10) {
			distanceCounter = 0;
			directionUp = !directionUp;
		}
	}
}
