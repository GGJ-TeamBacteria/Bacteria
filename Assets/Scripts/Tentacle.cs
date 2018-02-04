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
        // changes max length depends on player health
        CalcMaxLength();
        MoveHand();
        ManageAllTentacleFollowHandMovement();
    }

    public void CalcMaxLength()
    {
        if (isShrinking)
            return;
        //// if it is shrinking, don't extend
        //if (isShrinking)
        //{
        //    if (armParts.Count > currentMaxLength)
        //    {
        //        Shorten(1);
        //        return;
        //    } else {
        //        print("set isShrinking to false");
        //        isShrinking = false;
        //    }
        //}

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

            Transform spawnPoint = armParts[armParts.Count - 1].transform;
            Vector3 direction = Vector3.Normalize(spawnPoint.position - startOfTentacle);

            Extend(spawnPoint, direction);
        }
    }

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


    public void OnPlayerShrinkMortion()
    {
        autoExtend = false;
     //   print("set isShrinking true");
             isShrinking = true;
        //     currentMaxLength = 0;
        Shorten(armParts.Count);
        isShrinking = false;
    }

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
        currentSegment.gameObject.transform.SetParent(transform);

        armParts.Add(currentSegment);
        currentSegment.rootTentacle = this;

        // TENTACLES UP BUG: playerGameObject doesn't move so this isn't right
        // Is this how we introduced the bug when player moves away from 0,0,0?
        currentSegment.distanceFromPlayer = (currentSegment.transform.position - playerGameObject.transform.position).magnitude;
    }

    // TENTACLES UP BUG: even if we comment this method out, the bug still happens
    private void MoveHand()
    {
        if (armParts.Count == 0)
            return;

        if (controller == null)
            return;

        Vector3 direction = Vector3.Normalize(controller.position - transform.position);

        

        // multiply it with magnitude
        Vector3 newLoc = direction * armParts[0].distanceFromPlayer;

        armParts[0].transform.position = newLoc;

    }

    // TENTACLES UP BUG: even if we comment this method out, the bug still happens
    private void ManageAllTentacleFollowHandMovement()
    {
        
                if (armParts.Count == 0)
                    return;

                for (int i = 1; i < armParts.Count; i++)
                {
                    currentBodyPart = armParts[i];
                    prevBodyPart = armParts[i - 1];

                    // get normalized direction from player to prevbodypart
                    Vector3 direction = (prevBodyPart.transform.position - gameObject.transform.position).normalized;

                    // multiply it with magnitude
                    Vector3 newLoc = direction * currentBodyPart.distanceFromPlayer;

                    // follow it
                    currentBodyPart.transform.position = Vector3.Slerp(currentBodyPart.transform.position, newLoc, 0.3f);

                }
        
    }

}