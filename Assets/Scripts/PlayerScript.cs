using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    int m_health;


    // Use this for initialization
    void Start () {
        m_health = 3;
    }
	
	// Update is called once per frame
	void Update () {


        //If absorb -> gain 1 HP

        OnDeath();
    }

    void OnTriggerEnter(Collision other)
    {
        if (other.collider.CompareTag("Antibiotic"))
        {
            m_health = m_health - 2;
        }
        else //if (other.collider.CompareTag("Bacteria"))
        {
            m_health--;
        }

    }

    void OnDeath()
    {
        if (m_health <= 0)
        {
            //SceneManager.LoadScene("GameOver");
        }
    }
}
