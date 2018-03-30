using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceByTentacles : MonoBehaviour {
    public float strength = 100;

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
            GetComponent<Rigidbody>().velocity = forceVector * Time.deltaTime * strength * other.GetComponent<TentacleSegment>().getSpeed();
        }
    }
}
