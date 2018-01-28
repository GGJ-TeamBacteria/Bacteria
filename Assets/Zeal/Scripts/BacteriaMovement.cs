using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaMovement : MonoBehaviour {
	public float speed;
	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody>().velocity = transform.forward * Random.Range(-.5f, .5f) * speed;
		GetComponent<Rigidbody>().velocity = transform.right * Random.Range(-.5f, .5f) * speed;
		GetComponent<Rigidbody>().velocity = transform.up * Random.Range(-.5f, .5f) * speed;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void fixedUpdate() {
		
	}
}
