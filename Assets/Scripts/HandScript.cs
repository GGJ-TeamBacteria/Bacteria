using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Antibiotic"))
        {
            //Call TakeDamageAntibiotic() from Player
            //Retract Tentacle
        }
        else if (other.CompareTag("BadBacteria"))
        {
            //Call GainHealth() from Player
            //Call ExtendTentacle()
            Destroy(other.gameObject);
        }
        else // GoodBacteria
        {
            //Call GainHealth() from Player
            //Call ExtendTentacle()
            Destroy(other.gameObject);
        }

        
    }
}
