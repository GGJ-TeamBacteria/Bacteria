using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {

	public float tumble;

	// Update is called once per frame
	void Update () {
		transform.Rotate (new Vector3 (15, 15, 15) * tumble * Time.deltaTime);
	}
		
}
