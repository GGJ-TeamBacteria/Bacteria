﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSegment : MonoBehaviour {

    internal Tentacle rootTentacle;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Antibiotic"))
        {
            // Tell player to take damage from Antibiotic
            rootTentacle.playerRef.TakeDamageAntibiotic();
        }
        else if (other.CompareTag("BadBacteria"))
        {
            // Tell player to take damage from BadBacteria
            rootTentacle.playerRef.TakeDamageBacteria();
			SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
            //Destroy(other.gameObject);
        }
        else if (other.CompareTag("GoodBacteria"))
        {
            rootTentacle.buttonTrigger.Vibrate(500);

            //Call GainHealth() from Player
            rootTentacle.playerRef.GainHealth();
            SelfDestroy otherScript = other.GetComponent<SelfDestroy>();
            otherScript.Death();
            //Destroy(other.gameObject);
        }
        else if (other.CompareTag("HeartWin"))
        {
            rootTentacle.buttonTrigger.Vibrate(500);
            Destroy(other.gameObject);
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