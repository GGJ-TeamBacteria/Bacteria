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

        VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(_controller.controllerIndex), 1f, 0.5f, 0.01f);
    }

    void Update()
    {
        /*
                // For testing without a VR system
                if (Input.GetKeyDown("1"))
                {
                    leftTentacle.OnPlayerStretchMortion(vrSimulatorLeftHand.transform, vrSimulatorRightHand.transform.forward);
                }
                if (Input.GetKeyDown("2"))
                {
                    leftTentacle.OnPlayerShrinkMortion();
                }
                if (Input.GetKeyDown("3"))
                {
                    rightTentacle.OnPlayerStretchMortion(vrSimulatorRightHand.transform, vrSimulatorRightHand.transform.forward);
                }
                if (Input.GetKeyDown("4"))
                {
                    rightTentacle.OnPlayerShrinkMortion();
                }
        */
    }

    private void OnEnable()
    {
        _controller = GetComponent<SteamVR_TrackedController>();
        _controller.TriggerClicked += HandleTriggerClicked;
    }

    private void OnDisable()
    {
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