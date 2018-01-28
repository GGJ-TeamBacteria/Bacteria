using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public int m_health = 3;

	
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
        else if (other.CompareTag("Projectile"))
        {
            TakeDamageProjectile();
        }
        else
        {
            GainHealth();
        }

        // Other is destroyed -- handled in Bacteria Scripts
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
        //Play random damage sound
        m_health--;
    }

    public void TakeDamageAntibiotic()
    {
        //play antibiotic sound
        m_health = m_health - 2;
    }

    void TakeDamageProjectile()
    {
        //play projectile sound
        m_health = m_health - 1;
    }

    public int GetHealth()
    {
        return m_health;
    }
}
