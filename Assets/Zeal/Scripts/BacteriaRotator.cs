using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaRotator : MonoBehaviour {

	public float tumble;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (15, 15, 15) * Time.deltaTime);
	}
		
}
