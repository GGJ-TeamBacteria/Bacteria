﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceByTentacles : MonoBehaviour {

    public GameObject BounceSound1;
    public GameObject BounceSound2;

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
            switch(Random.Range(1, 3))
            {
                case 1:
                    if (BounceSound1 != null)
                    {
                        Instantiate(BounceSound1, transform.position, Quaternion.identity);
                    }
                    break;
                case 2:
                    if (BounceSound2 != null)
                    {
                        Instantiate(BounceSound2, transform.position, Quaternion.identity);
                    }
                    break;
              
            }
        }
    }
}
