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

    public Tentacle leftTentacle;
    public Tentacle rightTentacle;

    // Don't try to retract before we've ever extended
    private bool extendedAtLeastOnce = false;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Retract tentacle
    private void OnTriggerEnter(Collider collider)
    {
        if (!extendedAtLeastOnce)
            return;

        audioSource = GetComponent<AudioSource>();
        AudioClip retractSound = retractSounds[Random.Range(0, retractSounds.Length)];
        audioSource.PlayOneShot(retractSound);
 
        Tentacle tentacle = getTentacle(collider);
        if (tentacle != null)
        {
            print("GestureTrigger TriggerEnter retracting " + collider.name);
            tentacle.OnPlayerShrinkMortion();
        }
    }

    // Extend tentacle
    void OnTriggerExit(Collider collider)
    {
        // Avoid triggering in the first second of play
        //  if (Time.timeSinceLevelLoad < 1)
        //    return;

        // Trigger events are only sent if one of the Colliders also has a Rigidbody attached. Kinematic is OK

        extendedAtLeastOnce = true;

        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        audioSource.PlayOneShot(attackSound);

        Vector3 reachDirection = Vector3.Normalize(collider.bounds.center - playerBody.transform.position);

        Tentacle tentacle = getTentacle(collider);
        if (tentacle != null)
        {
            print("GestureTrigger TriggerExit reaching in direction: " + reachDirection +
                " for object name " + collider.name + " with tag " + collider.tag);
            tentacle.OnPlayerStretchMortion(collider.gameObject.transform, reachDirection);
        }
    }

    private Tentacle getTentacle(Collider collider)
    {
        if (collider.name == "ControllerChildLeft")
        {
            return leftTentacle;
        }
        else if (collider.name == "ControllerChildRight")
        {
            return rightTentacle;
        }
        else
        {
            // It can also collide with tentacle segments
            return null;
        }
    }


}
