using UnityEngine;
using VRTK;


// Tentacle extend/retract based on button presses
// Also handles vibration 
//
// Attach one of these to Controller (left) and one of these to Controller (right) underneath the steamvr [CameraRig] prefab

// Button detection based on https://unity3d.college/2016/11/16/steamvr-controller-input/

// This is called from TentacleSegment and PlayerScript to handle controller vibration

public class ButtonTrigger : MonoBehaviour
{
    public Tentacle tentacle;
    public AudioClip[] stretchAttackSounds;
    public AudioClip[] retractSounds;

    // This is just used for testing vibrations out
    public AudioClip[] vibrationTests;

    private AudioSource audioSource;

    private bool isExtended = false;

    private SteamVR_TrackedController _controller;

    // When player takes damage
    public void VibrateForSomethingBad()
    {
        Debug.Log("Vibrating controllers since player lost health");

        if (_controller == null)
        {
            Debug.Log("VibrateForSomethingBad not vibrating since no headset found");
            return;
        }

        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(_controller.controllerIndex), 1f, 0.4f, 0.001f);
    }


    // Trying to figure out how to use a poorly documented VRTK feature
    public void VibrateAudioClip(AudioClip audioClip)
    {
        // if running without headset
        if (_controller == null)
        {
            Debug.Log("not vibrating since no headset found");
            return;
        }

        Debug.Log("Vibration using audioclip " + audioClip.name);
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(_controller.controllerIndex), audioClip);
    }

    void Update()
    {
        // Make tentacle auto-extend at start of level
        if (!isExtended && Time.timeSinceLevelLoad > 0.5)
        {
            ExtendTentacle();
            isExtended = true;
        }

        // This is just used for testing vibrations out
        if (Input.GetKeyDown("1"))
        {
            VibrateForSomethingBad();
        }

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

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();

        // Commented out since we decided not to allow extending/retracting the tentacle
        // _controller.TriggerClicked += HandleTriggerClicked;
    }

    // We decided not to allow extending/retracting the tentacle
    //private void OnDisable()
    //{
    //    // if running without headset
    //    if (_controller == null)
    //        return;

    //    _controller.TriggerClicked -= HandleTriggerClicked;
    //}

    private void RetractTentacle()
    {
        // We decided not to allow retracting the tentacle 
        Debug.Log("tentacle retraction disabled");

        //audioSource = GetComponent<AudioSource>();
        //AudioClip retractSound = retractSounds[Random.Range(0, retractSounds.Length)];
        //audioSource.PlayOneShot(retractSound);

        //Debug.Log("ButtonTrigger retracting tentacle");
        //tentacle.OnPlayerShrinkMortion();
    }

    // We decided not to allow retracting the tentacle
    //private void HandleTriggerClicked(object sender, ClickedEventArgs e)
    //{
    //    Debug.Log("Controller trigger clicked");

    //    if (isExtended)
    //    {
    //        RetractTentacle();
    //        isExtended = false;
    //    }
    //    else
    //    {
    //        ExtendTentacle();
    //        isExtended = true;
    //    }
    //    //       spawnedPrimitive.transform.position = transform.position;
    //    //       spawnedPrimitive.transform.rotation = transform.rotation;
    //}
}