using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour {

    public int m_health = 3;
    AudioSource audioSource;

    public AudioClip player_lowhealth_loop;
    public AudioClip player_health_gain;
    public AudioClip player_health_lose;
    public AudioClip player_damage1;
    public AudioClip player_damage2;
    public AudioClip player_damage3;
    public AudioClip player_damage4;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (m_health == 1)
        {
            //loop stuff
        }
        else
        {
            //un loop stuff
        }

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
        audioSource.clip = player_health_gain;
        audioSource.Play();
        m_health++;
    }

    public void TakeDamageBacteria()
    {
        RandomDamageSound();
        m_health--;
    }

    public void TakeDamageAntibiotic()
    {
        RandomDamageSound();
        m_health = m_health - 2;
    }

    void TakeDamageProjectile()
    {
        RandomDamageSound();
        m_health--;
    }

    public int GetHealth()
    {
        return m_health;
    }

    void RandomDamageSound()
    {
        switch (Random.Range(1, 5))
        {
            case 1:
                audioSource.clip = player_damage1;
                break;
            case 2:
                audioSource.clip = player_damage2;
                break;
            case 3:
                audioSource.clip = player_damage3;
                break;
            case 4:
                audioSource.clip = player_damage4;
                break;
        }
        audioSource.Play();
    }
}
