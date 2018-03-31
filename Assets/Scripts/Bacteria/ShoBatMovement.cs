using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoBatMovement : MonoBehaviour {
	private Vector3 startingPosition;
	public int speed;
	private bool directionUp;
	private int distanceCounter = 0;

    
    public Transform center;
    public Vector3 axis = Vector3.right;
    public Vector3 desiredPosition;
    public float radius = 2.0f;
    public float radiusSpeed = 0.5f;
    public float rotationSpeed = 80.0f;

    // Use this for initialization
    void Start () {
        //startingPosition = transform.position;
        //directionUp = true;

        
        
        transform.position = (transform.position - center.position).normalized * radius + center.position;
        radius = 2.0f;
    }
	
	// Update is called once per frame
	void Update () {
        //if (directionUp) {
        //	transform.position = Vector3.MoveTowards (
        //		transform.position, 
        //		new Vector3 (startingPosition.x, startingPosition.y + 10, startingPosition.z), 
        //		speed * Time.deltaTime);
        //	distanceCounter++;
        //}
        //else {
        //	transform.position = Vector3.MoveTowards (
        //		transform.position, 
        //		new Vector3 (startingPosition.x, startingPosition.y - 10, startingPosition.z), 
        //		speed * Time.deltaTime);
        //	distanceCounter++;
        //}
        //if (distanceCounter >= 10) {
        //	distanceCounter = 0;
        //	directionUp = !directionUp;
        //}

        transform.RotateAround(center.position, axis, rotationSpeed * Time.deltaTime);
        desiredPosition = (transform.position - center.position).normalized * radius + center.position;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}
