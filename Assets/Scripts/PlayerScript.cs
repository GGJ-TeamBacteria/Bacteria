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

        OnDeath();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Antibiotic"))
        {
            TakeDamageAntibiotic();
        }
        else if (other.collider.CompareTag("BadBacteria"))
        {
            TakeDamageBacteria();
        }
        else
        {
            GainHealth();
        }

        Destroy(other.gameObject);
    }

    void OnDeath()
    {
        if (m_health <= 0)
        {
            //SceneManager.LoadScene("GameOver");
        }
    }

    void GainHealth()
    {
        m_health++;
    }

    void TakeDamageBacteria()
    {
        m_health--;
    }

    void TakeDamageAntibiotic()
    {
        m_health = m_health - 2;
    }
}
