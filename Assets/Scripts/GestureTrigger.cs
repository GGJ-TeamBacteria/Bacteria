using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GestureTrigger : MonoBehaviour
{
    public AudioClip[] stretchAttackSounds;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        // Trigger events are only sent if one of the Colliders also has a Rigidbody attached. Kinematic is OK
        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        audioSource.PlayOneShot(attackSound);
    }

    void OnTriggerStay(Collider other)
    {

    }
    void OnTriggerExit(Collider other)
    {

    }

    //GestureTriggerPrefab is a gameobject a prefab
    //GestureTrigger.cs
    //GestureTriggerColliderExample




}
