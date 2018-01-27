using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSegment : MonoBehaviour {

    internal float distanceFromPlayer;

}


//// exetend the arm
//TentacleSegment currentSegment = Instantiate(armPrefab, nextSpawnPoint.position + (direction * distanceOfTentacles), nextSpawnPoint.rotation);
//armParts.Add(currentSegment);
//            currentSegment.distanceFromPlayer = (currentSegment.transform.position - gameObject.transform.position).magnitude;


//if (Input.GetKey("2"))
//{
//    if (armParts.Count == 0)
//    {
//        return;
//    }

//    armParts.RemoveAt(armParts.Count-1);
//}