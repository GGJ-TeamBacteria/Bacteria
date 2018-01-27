using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour {

    public TentacleSegment armPrefab;
    public int maxArmLength;
    public float distanceOfTentacles;// check spell
    public float currentSpeed;
    public float rotationSpeed;
    private List<TentacleSegment> armParts;
    private TentacleSegment currentBodyPart;
    private TentacleSegment prevBodyPart;

    public float minDisBetweenArmParts;
    public float tenticalExtendingSpeed;

    // Use this for initialization
    void Start () {
        armParts = new List<TentacleSegment>();
		
	}
	
	// Update is called once per frame
	void Update () {

        // longer arm
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

            // exetend the arm
            TentacleSegment currentSegment = Instantiate(armPrefab, nextSpawnPoint.position + (direction * distanceOfTentacles), nextSpawnPoint.rotation);
            armParts.Add(currentSegment);
            currentSegment.distanceFromPlayer = (currentSegment.transform.position - gameObject.transform.position).magnitude;

        }

        if (Input.GetKey("2"))
        {
            if (armParts.Count == 0)
            {
                return;
            }

            TentacleSegment target = armParts[armParts.Count - 1];

            Destroy(target);
            armParts.Remove(target);
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            MoveHand();

        }

        ManageAllTentacleSegmentLocation();
        

    }

    private void MoveHand()
    {
        if (armParts.Count == 0)
            return;

        armParts[0].transform.Translate(armParts[0].transform.forward * currentSpeed * Time.smoothDeltaTime, Space.World);
        //if (Input.GetAxis("Horizontal") != 0)
        //{
        //    armParts[0].transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime * Input.GetAxis("Horizontal"));
        //}

    }

    private void ManageAllTentacleSegmentLocation()
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
