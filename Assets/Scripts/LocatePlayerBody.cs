using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocatePlayerBody : MonoBehaviour {

    public GameObject cameraHead;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Torso is below head
        transform.position = cameraHead.transform.position;
        transform.Translate(Vector3.down * 1, Space.World);
    }
}
