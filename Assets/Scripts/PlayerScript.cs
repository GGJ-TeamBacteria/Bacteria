using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour {

    public int m_health = 3;
    public int gainHealthPerBacteria = 2;
    AudioSource audioSource;
    AudioSource lowHealthAudioSource;

    public AudioClip player_health_gain;
    public AudioClip player_health_lose;
    public AudioClip player_damage1;
    public AudioClip player_damage2;
    public AudioClip player_damage3;
    public AudioClip player_damage4;
    public GameObject lowHealthWarning;

    // This is used for vibration so it can always refer to the SteamVR controller objects' ButtonTrigger.
    // When using the VRTK simulator, we don't need to point this to the simulated controllers' ButtonTrigger
    // since there is no simulation of vibration.
    public ButtonTrigger leftControllerButtonTrigger;
    public ButtonTrigger rightControllerButtonTrigger;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        lowHealthAudioSource = lowHealthWarning.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {

        if (m_health == 1 && !lowHealthAudioSource.isPlaying)
        {
                lowHealthAudioSource.Play();
        }
        else if (m_health > 1 && lowHealthAudioSource.isPlaying)
        {
            lowHealthAudioSource.Stop();
        }

        OnDeath();
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");

        if (other.CompareTag("Antibiotic"))
        {
            Debug.Log("Antibiotic");
            TakeDamageAntibiotic();

            leftControllerButtonTrigger.VibrateForSomethingBad();
            rightControllerButtonTrigger.VibrateForSomethingBad();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            Debug.Log("BadBacteria");
            TakeDamageBacteria();

            leftControllerButtonTrigger.VibrateForSomethingBad();
            rightControllerButtonTrigger.VibrateForSomethingBad();
        }
        else if (other.CompareTag("Projectile"))
        {
            Debug.Log("Projectile");
            TakeDamageProjectile();

            leftControllerButtonTrigger.VibrateForSomethingBad();
            rightControllerButtonTrigger.VibrateForSomethingBad();
        }
        else
        {
            Debug.Log("Health");
            GainHealth();

            leftControllerButtonTrigger.VibrateForSomethingGood();
            rightControllerButtonTrigger.VibrateForSomethingGood();
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
        m_health += gainHealthPerBacteria;
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
