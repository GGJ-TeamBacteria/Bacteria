using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GestureTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject playerBody;
    public AudioClip[] stretchAttackSounds;
    AudioSource audioSource;

    private Tentacle tentacle;

    // Use this for initialization
    void Start()
    {
        tentacle = player.GetComponent(typeof(Tentacle)) as Tentacle;
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


        // Trigger events are only sent if one of the Colliders also has a Rigidbody attached. Kinematic is OK
        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        audioSource.PlayOneShot(attackSound);

        Vector3 reachDirection = Vector3.Normalize(other.bounds.center - playerBody.transform.position);
        print("GestureTrigger TriggerExit reaching in direction: " + reachDirection); 

        tentacle.OnPlayerControllTenatacle(other.gameObject.transform, reachDirection);

    }


    //GestureTriggerPrefab is a gameobject a prefab
    //GestureTrigger.cs
    //GestureTriggerColliderExample




}
