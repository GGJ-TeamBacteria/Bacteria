using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GestureTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject playerBody;
    public AudioClip[] stretchAttackSounds;
    public AudioClip[] retractSounds;
    AudioSource audioSource;

    private Tentacle tentacle;

    // Don't try to retract before we've ever extended
    private bool extendedAtLeastOnce = false;

    // Use this for initialization
    void Start()
    {
        tentacle = player.GetComponent(typeof(Tentacle)) as Tentacle;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Retract tentacle
    private void OnTriggerEnter(Collider other)
    {
        if (!extendedAtLeastOnce)
            return;

        audioSource = GetComponent<AudioSource>();
        AudioClip retractSound = retractSounds[Random.Range(0, retractSounds.Length)];
        audioSource.PlayOneShot(retractSound);

        print("GestureTrigger TriggerEnter retracting");
        //tentacle.OnPlayerShrinkMortion(other.gameObject.transform);
    }

    // Extend tentacle
    void OnTriggerExit(Collider other)
    {
        // Avoid triggering in the first second of play
        //  if (Time.timeSinceLevelLoad < 1)
        //    return;

        // Trigger events are only sent if one of the Colliders also has a Rigidbody attached. Kinematic is OK

        extendedAtLeastOnce = true;

        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        audioSource.PlayOneShot(attackSound);

        Vector3 reachDirection = Vector3.Normalize(other.bounds.center - playerBody.transform.position);
        print("GestureTrigger TriggerExit reaching in direction: " + reachDirection); 

        tentacle.OnPlayerStretchMortion(other.gameObject.transform, reachDirection);

    }






}
