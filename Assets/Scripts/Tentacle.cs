﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour
{
    public TentacleSegment armPrefab;
    public int maxArmLength;
    public float distanceOfTentacles;
    public float currentSpeed;
    public float rotationSpeed;
    public float minDisBetweenArmParts;
    public float tenticalExtendingSpeed;
    public GameObject playerGameObject;
    public int superLongLength = 60;
    public int minLength = 10;

    public AudioClip[] stretchAttackSounds;

    // This is used for vibration so it can always refer to the SteamVR controller object's ButtonTrigger.
    // When using the VRTK simulator, we don't need to point this to the simulated controller's ButtonTrigger
    // since there is no simulation of vibration.
    public ButtonTrigger buttonTrigger;

    internal PlayerScript playerRef;
    private int currentMaxLength;
    private List<PoolableBehaviour> armParts;
    private Transform controller;
    private bool isShrinking;
    private bool autoExtend;
    private bool isExtendLengthRunning;

    // Use this for initialization
    void Start()
    {
        armParts = new List<PoolableBehaviour>();
        playerRef = playerGameObject.GetComponentInChildren<PlayerScript>();
        currentMaxLength = minLength;
    }

    // Update is called once per frame
    void Update()
    {
        // Change the max length depends on player health
        AdjustLength();

        // Move the first segment towards controller movement
        MoveHand();

        // Move all of the segments after the first one
        ManageAllTentacleFollowHandMovement();
    }

    /// <summary>
    ///  Change the max length depends on player health
    /// </summary>
    public void AdjustLength()
    {
        if (isShrinking)
            return;

        if (armParts.Count > currentMaxLength)
        {
            // Longer than max length
            Shorten(1);
        }
        else if (armParts.Count > 0 && armParts.Count < currentMaxLength)
        {
            // Shorter than current max length
            // spawn the segment after the furthest segment
            Transform spawnPoint = armParts[armParts.Count - 1].transform;
            Vector3 direction = controller.forward;
            Extend(spawnPoint, direction);
        }
        else if (armParts.Count > 0 && armParts.Count < minLength)
        {
            currentMaxLength = minLength;
        }
    }

    /// <summary>
    ///  Make the tentacle length shorter
    /// </summary>
    /// <param name="times">Define how short it should be</param>
    public void Shorten(int times)
    {
      //  print("shortening " + times + " times");
        for (int i = 0; i < times; i++)
        {
            if (armParts.Count > 1)
            {
                //   print("destroying arm segment");
                PoolableBehaviour target = armParts[armParts.Count - 1];
                armParts.Remove(target);
                target.SetActive(false);
            }
        }
    }

    /// <summary>
    ///  Receive the event which player try to extend their tentacle
    /// </summary>
    /// <param name="controllerLocation">Controller Transform</param>
    /// <param name="direction">Direction of the arm</param>
    public void OnPlayerStretchMortion(Transform controllerLocation, Vector3 direction)
    {
        autoExtend = true;
        if (armParts.Count != 0)
        {
            Debug.Log("ignoring player extend motion since tentacle already exists");
            return;
        }

        // keep the controller ref
        controller = controllerLocation;

        Extend(controller, direction);

    }

    /// <summary>
    ///  Receive the event which player try to shrink their tentacle
    /// </summary>
    public void OnPlayerShrinkMortion()
    {
        autoExtend = false;
        isShrinking = true;
        Shorten(armParts.Count);
        isShrinking = false;
    }

    /// <summary>
    ///  Spawn the tentacle parts
    /// </summary>
    /// <param name="spawnPoint">Tentacle root location</param>
    /// <param name="direction">Direction of the arm</param>
    private void Extend(Transform spawnPoint, Vector3 direction)
    {
        if (!autoExtend)
            return;

        //print("tenatcle Extend");
        //TentacleSegment currentSegment = Instantiate(armPrefab, GetSegmentLocation(spawnPoint.position, direction), spawnPoint.rotation);

        PoolableBehaviour currentSegment = PoolManager.instance.GetObjectFromPool(armPrefab);

        // reset the position of the obj
        currentSegment.transform.position = Vector3.zero;

        armParts.Add(currentSegment);
        (currentSegment as TentacleSegment).rootTentacle = this;

    }

    /// <summary>
    ///  Move the first segment of the tentacle towards player controller location
    /// </summary>
    private void MoveHand()
    {
        if (controller == null)
            return;

        if (armParts.Count == 0)
            return;

        armParts[0].transform.position = GetSegmentLocation(controller.position, controller.forward);
        armParts[0].transform.rotation = controller.rotation;

    }

    /// <summary>
    ///  Move all of the segments after the first one
    /// </summary>
    private void ManageAllTentacleFollowHandMovement()
    {
        if (controller == null)
            return;

        if (armParts.Count == 0)
            return;

        // Every segments follows previous one's direction
        // The first segment have already followed controller
        Transform prev = armParts[0].transform;
        for (int i = 1; i < armParts.Count; i++)
        {
            Vector3 current = armParts[i].transform.position;
            Vector3 disiredLocation = GetSegmentLocation(prev.position, (prev.position - controller.position).normalized);
            float dist = Vector3.Distance(current, disiredLocation);
            float alpha = 0.3f;

            if (dist > 0.5f)
            {
                alpha = 0.8f;
            }

            armParts[i].transform.position = Vector3.Slerp(current, disiredLocation, alpha);

            prev = armParts[i].transform;
        }
    }

    /// <summary>
    ///  Utility function for calculate segment location
    /// </summary>
    /// <param name="prevLocation">The location of the previous segment</param>
    /// <param name="direction">Direction of the segment</param>
    private Vector3 GetSegmentLocation(Vector3 prevLocation, Vector3 direction)
    {
        return prevLocation + direction * distanceOfTentacles;
    }

    // Power UP

    public void ExtendMaxLength(int length)
    {
        // extend the tentacle parmanently
        maxArmLength += length;

        // extend the tentacle current length as well ( it can be longer than max length when it got power up )
        currentMaxLength += length;
    }

    public void PowerUpSuper(float duration) {

        AudioClip attackSound = stretchAttackSounds[Random.Range(0, stretchAttackSounds.Length)];
        GetComponent<AudioSource>().PlayOneShot(attackSound);

        if (!isExtendLengthRunning)
            StartCoroutine("ExtendLengthTemporary", duration);
    }
    public void PowerUpHealth(int health) {
        playerRef.GainHealth(health);
    }

    IEnumerator ExtendLengthTemporary(float duration)
    {
        isExtendLengthRunning = true;
        currentMaxLength = superLongLength;
        yield return new WaitForSeconds(duration);
        currentMaxLength = maxArmLength;
        isExtendLengthRunning = false;
    }

}