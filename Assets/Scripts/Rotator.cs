using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

    Vector3 origin = transform.position;

	public float tumble;
    void Start()
    {

    }

	// Update is called once per frame
	void Update () {
		transform.RotateAround (origin, Vector3.right, tumble * Time.deltaTime);
	}
		
}
