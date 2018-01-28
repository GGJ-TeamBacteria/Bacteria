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

    internal PlayerScript playerRef;
    private List<TentacleSegment> armParts;
    private TentacleSegment currentBodyPart;
    private TentacleSegment prevBodyPart;

    // Use this for initialization
    void Start()
    {
        armParts = new List<TentacleSegment>();
        playerRef = gameObject.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {

        // extending arm
        if (Input.GetKey("1"))
        {

            if (armParts.Count >= maxArmLength)
            {
                return;
            }

            GameObject spawnPoint = GameObject.FindWithTag("TentacleSpawnPoint");
            Transform nextSpawnPoint = spawnPoint.transform;
            if (armParts.Count > 0)
            {
                nextSpawnPoint = armParts[armParts.Count - 1].transform;
            }

            Vector3 heading = nextSpawnPoint.position - transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            TentacleSegment currentSegment = Instantiate(armPrefab, nextSpawnPoint.position + (direction * distanceOfTentacles), nextSpawnPoint.rotation);
            currentSegment.gameObject.transform.SetParent(transform);
            armParts.Add(currentSegment);
            currentSegment.distanceFromPlayer = (currentSegment.transform.position - gameObject.transform.position).magnitude;
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
            MoveHand();

        }

        ManageAllTentacleFollowHandMovement();
    }

    public void OnPlayerControllTenatacle(Transform controllerLocation, Vector3 direction)
    {
        if (armParts.Count >= maxArmLength)
        {
            return;
        }

        //GameObject spawnPoint = GameObject.FindWithTag("TentacleSpawnPoint");
        //Transform nextSpawnPoint = spawnPoint.transform;

        Transform nextSpawnPoint = controllerLocation;
        if (armParts.Count > 0)
        {
            nextSpawnPoint = armParts[armParts.Count - 1].transform;
        }

        //Vector3 heading = nextSpawnPoint.position - transform.position;
        //var distance = heading.magnitude;
        //var headingDirection = heading / distance; // This is now the normalized direction.

        TentacleSegment currentSegment = Instantiate(armPrefab, nextSpawnPoint.position + (direction * distanceOfTentacles), nextSpawnPoint.rotation);
        currentSegment.gameObject.transform.SetParent(transform);
        armParts.Add(currentSegment);
        currentSegment.distanceFromPlayer = (currentSegment.transform.position - gameObject.transform.position).magnitude;
    }

    public void TakeDamage()
    {

        // get new health from player to set max tentacle length
        // Shorten tentacle

        // Change the name
        // do this in update
    }

    private void MoveHand()
    {
        if (armParts.Count == 0)
            return;

        armParts[0].transform.Translate(armParts[0].transform.forward * currentSpeed * Time.smoothDeltaTime, Space.World);

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
            Vector3 dis = (prevBodyPart.transform.position - gameObject.transform.position).normalized;

            // multiply it with magnitude
            Vector3 newLoc = dis * currentBodyPart.distanceFromPlayer;

            // follow it
            currentBodyPart.transform.position = Vector3.Slerp(currentBodyPart.transform.position, newLoc, 0.3f);

        }
    }
}