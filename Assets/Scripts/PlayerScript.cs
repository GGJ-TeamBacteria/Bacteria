using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {

    public int m_health = 3;
    AudioSource audioSource;
    AudioSource lowHealthAudioSource;

    public AudioClip player_lowhealth_loop;
    public AudioClip player_health_gain;
    public AudioClip player_health_lose;
    public AudioClip player_damage1;
    public AudioClip player_damage2;
    public AudioClip player_damage3;
    public AudioClip player_damage4;
    public GameObject lowHealthWarning;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowHealthAudioSource = lowHealthWarning.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {

        if (m_health == 1)
        {
            lowHealthAudioSource.Play();
        }
        else if (m_health > 1)
        {
            lowHealthAudioSource.Stop();
        }

        OnDeath();
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");

        if (other.CompareTag("Antibiotic"))
        {
            Debug.Log("Antibiotic");
            TakeDamageAntibiotic();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            Debug.Log("Bacteria");
            TakeDamageBacteria();
        }
        else if (other.CompareTag("Projectile"))
        {
            Debug.Log("Projectile");
            TakeDamageProjectile();
        }
        else
        {
            Debug.Log("Health");
            GainHealth();
        }

        // Other is destroyed -- handled in Bacteria Scripts
    }

    void OnDeath()
    {
        if (m_health <= 0)
        {
            SceneManager.LoadScene("GameOver");
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
        m_health--;
        Debug.Log("take damage");
        RandomDamageSound();
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
                Debug.Log("DMG1");
                break;
            case 2:
                audioSource.clip = player_damage2;
                Debug.Log("DMG2");
                break;
            case 3:
                audioSource.clip = player_damage3;
                Debug.Log("DMG3");
                break;
            case 4:
                audioSource.clip = player_damage4;
                Debug.Log("DMG4");
                break;
        }
        audioSource.Play();
    }
}
