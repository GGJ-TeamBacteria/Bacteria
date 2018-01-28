using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaMovement : MonoBehaviour {
	public float speed;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().velocity = new Vector3 (
			Random.Range (-.5f, .5f) * speed,
			Random.Range (-.5f, .5f) * speed,
			Random.Range (-.5f, .5f) * speed
		);
	
	}
}
