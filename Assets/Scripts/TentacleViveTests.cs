using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleViveTests : MonoBehaviour
{

    public TentacleSegment armPrefab;
    public int maxArmLength;
    public float distanceOfTentacles;
    public float currentSpeed;
    public float rotationSpeed;
    public float minDisBetweenArmParts;
    public float tenticalExtendingSpeed;

    public GameObject playerLocationReference;
    public GameObject playerGameObject;
    internal PlayerScript playerRef;
    private int currentMaxLength;
    private List<TentacleSegment> armParts;
    private TentacleSegment currentBodyPart;
    private TentacleSegment prevBodyPart;
    private Transform controller;
    private bool isShrinking;
    private bool autoExtend;
    private Vector3 currentDirection;

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

        // extending arm
        if (Input.GetKeyDown("1"))
        {

            if (armParts.Count >= currentMaxLength)
            {
                return;
            }

            GameObject spawnPoint = GameObject.FindWithTag("TentacleSpawnPoint");
            Transform nextSpawnPoint = spawnPoint.transform;
            if (armParts.Count > 0)
            {
                nextSpawnPoint = armParts[armParts.Count - 1].transform;
            }

            Vector3 heading = nextSpawnPoint.position - playerLocationReference.transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            TentacleSegment currentSegment = Instantiate(armPrefab, nextSpawnPoint.position + (direction * distanceOfTentacles), nextSpawnPoint.rotation);
            currentSegment.gameObject.transform.SetParent(transform);
            armParts.Add(currentSegment);
            currentSegment.rootTentacle = this;
            currentSegment.distanceFromPlayer = (currentSegment.transform.position - playerLocationReference.transform.position).magnitude;
        }

        // Shorten Arm
        if (Input.GetKey("2"))
        {
            if (armParts.Count == 0)
            {
                return;
            }

            TentacleSegment target = armParts[armParts.Count - 1];

            armParts.Remove(target);
            Destroy(target.gameObject);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            //MoveHand();
        }
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
            Vector3 direction = Vector3.Normalize(spawnPoint.position - playerLocationReference.transform.position);

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
            return;

        // spawn the first one
        controller = controllerLocation;

        Transform spawnPoint = controllerLocation;


        Extend(spawnPoint, direction, true);

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

    private void Extend(Transform spawnPoint, Vector3 direction, bool firstSegment = false)
    {
        if (!autoExtend)
            return;

        // print("tenatcle Extend");
        TentacleSegment currentSegment = Instantiate(armPrefab, spawnPoint.position + (direction * distanceOfTentacles), spawnPoint.rotation);
        currentSegment.gameObject.transform.SetParent(transform);
        string model = UnityEngine.XR.XRDevice.model != null ? UnityEngine.XR.XRDevice.model : "";
        print("model = '" + model + "'");
        if (firstSegment && model.IndexOf("Vive") >= 0)
        {
            print("vive detected - compensating segment positoin");
            currentSegment.transform.Translate(Vector3.up * 0.5f, Space.World);
        }

        armParts.Add(currentSegment);
        currentSegment.rootTentacle = this;
        currentSegment.distanceFromPlayer = (currentSegment.transform.position - playerLocationReference.transform.position).magnitude;
    }

    private void MoveHand()
    {
        if (armParts.Count == 0)
            return;

        if (controller == null)
            return;

        Vector3 direction = Vector3.Normalize(controller.position - playerLocationReference.transform.position);

        currentDirection = direction;

        // multiply it with magnitude
        Vector3 newLoc = direction * armParts[0].distanceFromPlayer;

        armParts[0].transform.position = newLoc;

    }

    private void ManageAllTentacleFollowHandMovement()
    {
        if (armParts.Count == 0)
            return;

        for (int i = 1; i < armParts.Count; i++)
        {
            currentBodyPart = armParts[i];
            prevBodyPart = armParts[i - 1];

            // get normalized direction from player to prevbodypart
            //Vector3 direction = (prevBodyPart.transform.position - gameObject.transform.position).normalized;
            //Vector3 direction = (prevBodyPart.transform.position - playerLocationReference.transform.position).normalized;
            Vector3 direction = currentDirection;
            // multiply it with magnitude
            Vector3 newLoc = direction * currentBodyPart.distanceFromPlayer;

            // follow it
            currentBodyPart.transform.position = Vector3.Slerp(currentBodyPart.transform.position, newLoc, 0.3f);

        }
    }
}