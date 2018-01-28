using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSegment : MonoBehaviour {

    internal float distanceFromPlayer;
    internal Tentacle rootTentacle;

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Tentacle collided to " + other);

        if (other.CompareTag("Antibiotic"))
        {
            // Tell player to take damage from Antibiotic
            rootTentacle.playerRef.TakeDamageAntibiotic();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            // Tell player to take damage from BadBacteria
            rootTentacle.playerRef.TakeDamageBacteria();
            BacteriaDestroy otherScript = other.GetComponent<BacteriaDestroy>();
            otherScript.Death();
            //Destroy(other.gameObject);
        }
        else if (other.CompareTag("GoodBacteria"))
        {
            //Call GainHealth() from Player
            rootTentacle.playerRef.GainHealth();

            //Destroy(other.gameObject);
        }
    }
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