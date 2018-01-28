using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GestureTrigger : MonoBehaviour
{
    public GameObject playerBody;
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

    void OnTriggerExit(Collider other)
    {
        // Avoid triggering in the first second of play
      //  if (Time.timeSinceLevelLoad < 1)
        //    return;


        print("GestureTrigger OnTriggerExit reached");
        // Trigger events are only sent if one of the Colliders also has a Rigidbody attached. Kinematic is OK
        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        audioSource.PlayOneShot(attackSound);

        Vector3 reachDirection = Vector3.Normalize(other.bounds.center - playerBody.transform.position);
        print(reachDirection);
        // world coordinate unit vector
        //OnPlayerControllTenatacle(Transform controllerLocation, Vector3 direction)

    }


    //GestureTriggerPrefab is a gameobject a prefab
    //GestureTrigger.cs
    //GestureTriggerColliderExample




}
