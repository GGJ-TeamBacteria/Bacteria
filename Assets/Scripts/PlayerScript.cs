using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public int m_health = 3;


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        OnDeath();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Antibiotic"))
        {
            TakeDamageAntibiotic();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            TakeDamageBacteria();
        }
        else
        {
            GainHealth();
        }

        // Destroy(other.gameObject);
    }

    void OnDeath()
    {
        if (m_health <= 0)
        {
            //SceneManager.LoadScene("GameOver");
        }
    }

    public void GainHealth()
    {
        m_health++;
    }

    public void TakeDamageBacteria()
    {
        m_health--;
    }

    public void TakeDamageAntibiotic()
    {
        m_health = m_health - 2;
    }

    public int GetHealth()
    {
        return m_health;
    }
}
