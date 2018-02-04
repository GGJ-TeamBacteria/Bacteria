using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GestureTrigger : MonoBehaviour
{
    public GameObject player;
    public GameObject playerBody;
    public GameObject vrSimulatorCameraRig;

    public AudioClip[] stretchAttackSounds;
    public AudioClip[] retractSounds;
    AudioSource audioSource;

    public Tentacle leftTentacle;
    public Tentacle rightTentacle;

    // Don't try to retract before we've ever extended
    private bool extendedAtLeastOnce = false;

    public bool assumeVive = false;
    private bool vive = false;

    // Use this for initialization
    void Start()
    {
        vive = assumeVive;
        string model = UnityEngine.XR.XRDevice.model != null ? UnityEngine.XR.XRDevice.model : "";
        print("model = '" + model + "'");
        if (model.IndexOf("Vive") >= 0)
        {
                print("vive detected");
                vive = true;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        // For testing without a VR system
        if (Input.GetKeyDown("1"))
        {
            leftTentacle.OnPlayerStretchMortion(vrSimulatorCameraRig.transform, vrSimulatorCameraRig.transform.forward);
        }
        if (Input.GetKeyDown("2"))
        {
            leftTentacle.OnPlayerShrinkMortion();
        }
        if (Input.GetKeyDown("3"))
        {
            rightTentacle.OnPlayerStretchMortion(vrSimulatorCameraRig.transform, vrSimulatorCameraRig.transform.forward);
        }
        if (Input.GetKeyDown("4"))
        {
            rightTentacle.OnPlayerShrinkMortion();
        }

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
    // Triggered when player reaches far enough out to cross the surface of the
    // capsule collider around the player.
    void OnTriggerExit(Collider collider)
    {

        // Trigger events are only sent if one of the Colliders also has a Rigidbody attached. Kinematic is OK

        extendedAtLeastOnce = true;

        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        audioSource.PlayOneShot(attackSound);

        // Original approach:
        //
        // Find direction of reach using a ROUGH GUESS of where the player's body center is.
        // - This is a flawed approach because the player may be reaching in an arc that
        //   starts somewhere other than their body center.
        // - We are only making a crude guess where the body center is.
        // Vector3 controllerCenter = collider.bounds.center;
        // Vector3 reachDirection = Vector3.Normalize(controllerCenter - playerBody.transform.position);

        // Reach direction = direction controller is pointing
        Vector3 reachDirection = Vector3.Normalize(collider.transform.forward);

        // TENTACLES UP BUG: interesting to see what happens if we force this
        //reachDirection = new Vector3(0.3f, 0.0f, 0.3f);

        Tentacle tentacle = getTentacle(collider);
        if (tentacle != null)
        {
            //print("GestureTrigger TriggerExit reaching in direction: " + reachDirection +
            //    " for object name " + collider.name + " with tag " + collider.tag);
            DebugDrawLine(collider.gameObject.transform.position,
                collider.gameObject.transform.position + reachDirection, Color.red, 3f);

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

    // https://answers.unity.com/answers/1108340/view.html
    void DebugDrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = Color.white;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

}
