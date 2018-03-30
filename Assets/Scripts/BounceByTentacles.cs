using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceByTentacles : MonoBehaviour {
    float strength = 100;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    //detect collison
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Tentacle")
        {
            Debug.Log("Projectil hit tentacles");
            Vector3 forceVector = GetComponent<Transform>().position - other.GetComponent<Transform>().position;
            Vector3 forceUnitVector = forceVector.normalized();
            GetComponent<Rigidbody>().velocity = forceUnitVector * Time.deltaTime * strength * other.getSpeed();
        }
    }
}
