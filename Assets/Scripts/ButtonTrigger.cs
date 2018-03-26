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


    // lengthInMicroseconds may need to be in the range 1-3999 according to answers.unity.com
    // but if i look into how TriggerHapticPulse is defined, it seems to actually get cast to a (char) 
    public void Vibrate(ushort lengthInMicroseconds)
    {
        // if running without headset
        if (_controller == null)
            return;

        Debug.Log("Vibration for " + (lengthInMicroseconds / 1000f) + " milliseconds");
        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(_controller.controllerIndex), 1f, 0.5f, 0.01f);
    }

    public void VibrateAudioClip(AudioClip audioClip)
    {
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

        // if running without headset
        if (_controller == null)
            return;

        // This is just used for testing vibrations out
        if (Input.GetKeyDown("="))
        {
            Vibrate(500);
        }
        else if (Input.GetKeyDown("1"))
        {
            VibrateAudioClip(vibrationTests[0]);
        }
        else if (Input.GetKeyDown("2"))
        {
            VibrateAudioClip(vibrationTests[1]);
        }
        else if (Input.GetKeyDown("3"))
        {
            VibrateAudioClip(vibrationTests[2]);
        }
        else if (Input.GetKeyDown("4"))
        {
            VibrateAudioClip(vibrationTests[3]);
        }
        else if (Input.GetKeyDown("5"))
        {
            VibrateAudioClip(vibrationTests[4]);
        }
        else if (Input.GetKeyDown("6"))
        {
            VibrateAudioClip(vibrationTests[5]);
        }
        else if (Input.GetKeyDown("7"))
        {
            VibrateAudioClip(vibrationTests[6]);
        }
        else if (Input.GetKeyDown("8"))
        {
            VibrateAudioClip(vibrationTests[7]);
        }
        else if (Input.GetKeyDown("9"))
        {
            VibrateAudioClip(vibrationTests[8]);
        }
        else if (Input.GetKeyDown("0"))
        {
            VibrateAudioClip(vibrationTests[9]);
        }
        else if (Input.GetKeyDown("-"))
        {
            VibrateAudioClip(vibrationTests[10]);
        }
        else if (Input.GetKeyDown("\\"))
        {
            VibrateAudioClip(vibrationTests[11]);
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

    // We decided not to allow extending/retracting the tentacle
    //private void OnEnable()
    //{
    //    // if running without headset
    //    if (_controller == null)
    //        return;

    //    _controller = GetComponent<SteamVR_TrackedController>();
    //    _controller.TriggerClicked += HandleTriggerClicked;
    //}

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