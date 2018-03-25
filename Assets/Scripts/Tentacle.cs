using System.Collections;
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

    public ButtonTrigger buttonTrigger;

    internal PlayerScript playerRef;
    private int currentMaxLength;
    private List<TentacleSegment> armParts;
    private Transform controller;
    private bool isShrinking;
    private bool autoExtend;

    // Use this for initialization
    void Start()
    {
        armParts = new List<TentacleSegment>();
        playerRef = playerGameObject.GetComponentInChildren<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // Change the max length depends on player health
        CalcMaxLength();

        // Move the first segment towards controller movement
        MoveHand();

        // Move all of the segments after the first one
        ManageAllTentacleFollowHandMovement();
    }

    /// <summary>
    ///  Change the max length depends on player health
    /// </summary>
    public void CalcMaxLength()
    {
        if (isShrinking)
            return;

        int currentHealth = playerRef.GetHealth();

        int nextMaxLength = currentHealth * 2;
        if (nextMaxLength <= maxArmLength)
        {
            currentMaxLength = nextMaxLength;
        }

        // Longer than max length
        if (armParts.Count > currentMaxLength)
        {
            Shorten(1);
        }

        // Shorter than max length
        if (armParts.Count > 0 && armParts.Count < currentMaxLength)
        {
            // spawn the segment after the furthest segment
            Transform spawnPoint = armParts[armParts.Count - 1].transform;
            Vector3 direction = controller.forward;
            Extend(spawnPoint, direction);
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
                TentacleSegment target = armParts[armParts.Count - 1];

                armParts.Remove(target);
                Destroy(target.gameObject);
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
        //   print("set isShrinking true");
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
        TentacleSegment currentSegment = Instantiate(armPrefab, GetSegmentLocation(spawnPoint.position, direction), spawnPoint.rotation);
        armParts.Add(currentSegment);
        currentSegment.rootTentacle = this;

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
            armParts[i].transform.position = Vector3.Slerp(armParts[i].transform.position, 
                GetSegmentLocation(prev.position, (prev.position - controller.position).normalized), 0.3f);

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
}