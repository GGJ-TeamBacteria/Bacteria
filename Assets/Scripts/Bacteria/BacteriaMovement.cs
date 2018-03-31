using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaMovement : MonoBehaviour {
	public float speed;
    AudioSource audioSource;
    public AudioClip player_damage1;
    public AudioClip player_damage2;
    public AudioClip player_damage3;
    public AudioClip player_damage4;
    // Use this for initialization
    void Start ()
    {
		GetComponent<Rigidbody> ().velocity = new Vector3 (
			Random.Range (-.5f, .5f) * speed,
			Random.Range (-.5f, .5f) * speed,
			Random.Range (-.5f, .5f) * speed
		);

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        
    }

    void RandomSound()
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
