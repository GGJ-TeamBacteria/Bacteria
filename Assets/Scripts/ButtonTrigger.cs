using UnityEngine;
using VRTK;


// Tentacle extend/retract based on button presses
// Also handles vibration 
//
// Attach one of these to Controller (left) and one of these to Controller (right) underneath the steamvr [CameraRig] prefab

// Button detection based on https://unity3d.college/2016/11/16/steamvr-controller-input/

public class ButtonTrigger : MonoBehaviour
{
    public Tentacle tentacle;
    public AudioClip[] stretchAttackSounds;
    public AudioClip[] retractSounds;

    private AudioSource audioSource;

    private bool isExtended = false;

    private SteamVR_TrackedController _controller;


    // lengthInMicroseconds may need to be in the range 1-3999 according to answers.unity.com
    // but if i look into how TriggerHapticPulse is defined, it seems to actually get cast to a (char) 
    public void Vibrate(ushort lengthInMicroseconds)
    {
        // if running without headset
        if (_controller == null)
            return;

        // There are lots of variations on TriggerHapticPulse
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(_controller.controllerIndex), 1f, 0.5f, 0.01f);
    }


    private void Start()
    {        
    }

    void Update()
    {
        // For testing without a VR system
        if (Input.GetKeyDown("1"))
        {
            Debug.Log("extend tentacle based on keypress 1");
            ExtendTentacle();
        }
        else if (Input.GetKeyDown("2"))
        {
            Debug.Log("retract tentacle based on keypress 2");
            RetractTentacle();
        }
    }

    private void OnEnable()
    {
        // if running without headset
        if (_controller == null)
            return;

        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.TriggerClicked += HandleTriggerClicked;
    }

    private void OnDisable()
    {
        // if running without headset
        if (_controller == null)
            return;

        _controller.TriggerClicked -= HandleTriggerClicked;
    }

    private void ExtendTentacle()
    {
        audioSource = GetComponent<AudioSource>();
        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        audioSource.PlayOneShot(attackSound);

        // Tentacle will point in direction that the controller is pointing.
        Vector3 reachDirection = Vector3.Normalize(transform.forward);

        Debug.Log("ButtonTrigger extending tentacle");
        tentacle.OnPlayerStretchMortion(transform, reachDirection);
    }

    private void RetractTentacle()
    {
        audioSource = GetComponent<AudioSource>();
        AudioClip retractSound = retractSounds[Random.Range(0, retractSounds.Length)];
        audioSource.PlayOneShot(retractSound);

        Debug.Log("ButtonTrigger retracting tentacle");
        tentacle.OnPlayerShrinkMortion();
    }

    private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    {
        Debug.Log("Controller trigger clicked");

        if (isExtended)
        {
            RetractTentacle();
            isExtended = false;
        }
        else
        {
            ExtendTentacle();
            isExtended = true;
        }
        //       spawnedPrimitive.transform.position = transform.position;
        //       spawnedPrimitive.transform.rotation = transform.rotation;
    }
}