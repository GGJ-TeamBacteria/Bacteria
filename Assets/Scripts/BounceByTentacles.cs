using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceByTentacles : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //detect collison
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tentacles")
        {
            Debug.Log("Projectil hit tentacles");
            GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity * -1;
        }
    }
}
