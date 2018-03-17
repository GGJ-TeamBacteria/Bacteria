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
    internal PlayerScript playerRef;
    private int currentMaxLength;
    private List<TentacleSegment> armParts;
    private TentacleSegment currentBodyPart;
    private TentacleSegment prevBodyPart;
    private Transform controller;
    private bool isShrinking;
    private bool autoExtend;

    private Vector3 startOfTentacle;

    // Use this for initialization
    void Start()
    {
        armParts = new List<TentacleSegment>();
        playerRef = playerGameObject.GetComponent<PlayerScript>();
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

            // TODO:
            //Transform spawnPoint = armParts[armParts.Count - 1].transform;
            //Vector3 direction = Vector3.Normalize(spawnPoint.position - startOfTentacle);

            Vector3 direction = (controller.transform.rotation * Vector3.forward).normalized;
            Extend(controller.transform, direction);
        }
    }

    /// <summary>
    ///  Make the tentacle length shorter
    /// </summary>
    /// <param name="times">Define how short it should be</param>
    public void Shorten(int times)
    {
      //  print("shortening " + times + " times");
        for (int i = 0; i < times && i < armParts.Count; i++)
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

        // spawn the first one
        controller = controllerLocation;

        Transform spawnPoint = controllerLocation;
        startOfTentacle = spawnPoint.position;


        // TENTACLES UP BUG: it seems like because of the bug, it makes no difference
        // what "direction" is here. we can force a value here like
        // "direction = new Vector3(0.3f, 0, 0.3f)" and the
        // tentacles will look the same as before.

        Extend(spawnPoint, direction);

    }

    /// <summary>
    ///  Receive the event which player try to shrink their tentacle
    /// </summary>
    public void OnPlayerShrinkMortion()
    {
        autoExtend = false;
        //   print("set isShrinking true");
        isShrinking = true;
        //     currentMaxLength = 0;
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

        // print("tenatcle Extend");
        TentacleSegment currentSegment = Instantiate(armPrefab, spawnPoint.position + (direction * distanceOfTentacles), spawnPoint.rotation);

        // TENTACLES UP BUG: try adding DebugDrawLine (from GestureTrigger.cs) to show why the object
        // is instantiated where it is (spawnPoint.position, direction, etc.) 

        // TENTACLES UP BUG: is this right? 
        // The GameObject that object Tentacle.cs is attached to always stays at 0,0,0 in world space
        // Is this how we introduced the bug when player moves away from 0,0,0?
        currentSegment.gameObject.transform.SetParent(controller.transform);

        armParts.Add(currentSegment);
        currentSegment.rootTentacle = this;

        // TENTACLES UP BUG: playerGameObject doesn't move so this isn't right
        // Is this how we introduced the bug when player moves away from 0,0,0?

        // TODO: may be I need to store distance from controller location
        currentSegment.distanceFromPlayer = (currentSegment.transform.position - controller.transform.position).magnitude;
    }

    // TENTACLES UP BUG: even if we comment this method out, the bug still happens
    /// <summary>
    ///  Move the first segment of the tentacle towards player controller location
    /// </summary>
    private void MoveHand()
    {
        if (controller == null)
            return;

        if (armParts.Count == 0)
            return;


        // TODO
        //Vector3 direction = Vector3.Normalize(controller.position - transform.position);
        //Vector3 direction = (controller.transform.rotation * Vector3.forward).normalized;


        // multiply it with segment magnitude
        //Vector3 newLoc = direction * armParts[0].distanceFromPlayer;

        armParts[0].transform.position = controller.position;

    }

    // TENTACLES UP BUG: even if we comment this method out, the bug still happens
    /// <summary>
    ///  Move all of the segments after the first one
    /// </summary>
    private void ManageAllTentacleFollowHandMovement()
    {
        if (controller == null)
            return;

        if (armParts.Count == 0)
            return;

        for (int i = 1; i < armParts.Count; i++)
        {
            currentBodyPart = armParts[i];
            prevBodyPart = armParts[i - 1];

            // get normalized direction from player to prevbodypart
            Vector3 direction = (prevBodyPart.transform.position - controller.transform.position).normalized;

            // multiply it with magnitude
            Vector3 newLoc = direction * currentBodyPart.distanceFromPlayer;

            // follow it
            currentBodyPart.transform.position = Vector3.Slerp(currentBodyPart.transform.position, newLoc, 0.3f);

        }
        
    }

}