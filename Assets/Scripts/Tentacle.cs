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

            Vector3 heading = nextSpawnPoint.position - transform.position;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            TentacleSegment currentSegment = Instantiate(armPrefab, nextSpawnPoint.position + (direction * distanceOfTentacles), nextSpawnPoint.rotation);
            currentSegment.gameObject.transform.SetParent(transform);
            armParts.Add(currentSegment);
            currentSegment.rootTentacle = this;
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
            Vector3 direction = Vector3.Normalize(spawnPoint.position - transform.position);

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

        DebugDrawLine(spawnPoint.position, spawnPoint.position + direction, Color.red, 4f);


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

    // https://answers.unity.com/answers/1108340/view.html
    void DebugDrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
    {
        GameObject myLine = new GameObject();
        myLine.transform.position = start;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        GameObject.Destroy(myLine, duration);
    }

    private void Extend(Transform spawnPoint, Vector3 direction)
    {
        if (!autoExtend)
            return;

      // print("tenatcle Extend");
        TentacleSegment currentSegment = Instantiate(armPrefab, spawnPoint.position + (direction * distanceOfTentacles), spawnPoint.rotation);
        currentSegment.gameObject.transform.SetParent(transform);
        armParts.Add(currentSegment);
        currentSegment.rootTentacle = this;
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