﻿using System.Collections;
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
    private int currentMaxLength;
    private List<TentacleSegment> armParts;
    private TentacleSegment currentBodyPart;
    private TentacleSegment prevBodyPart;
    private Vector3 currentDirection;
    private Transform controller;

    // Use this for initialization
    void Start()
    {
        armParts = new List<TentacleSegment>();
        playerRef = gameObject.GetComponent<PlayerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        // changes max length depends on player health
        CalcMaxLength();

        // extending arm
        if (Input.GetKey("1"))
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

        MoveHand();
        ManageAllTentacleFollowHandMovement();
    }

    public void CalcMaxLength()
    {
        int currentHealth = playerRef.GetHealth();
        currentMaxLength = currentHealth * 2;

        if (armParts.Count > currentMaxLength)
        {
            Shorten(1);
        }
    }

    public void Shorten(int times)
    {
        for (int i = 0; i < times || i <= armParts.Count; i++)
        {
            TentacleSegment target = armParts[armParts.Count - 1];

            armParts.Remove(target);
            Destroy(target.gameObject);
        }

    }

    public void OnPlayerControllTenatacle(Transform controllerLocation, Vector3 direction)
    {
        if (armParts.Count > currentMaxLength)
        {
            return;
        }

        controller = controllerLocation;

        Transform spawnPoint = controllerLocation;

        // If there is already tentacle, spawn new one from there
        if (armParts.Count > 0)
        {
            spawnPoint = armParts[armParts.Count - 1].transform;
        }

        TentacleSegment currentSegment = Instantiate(armPrefab, spawnPoint.position + (direction * distanceOfTentacles), spawnPoint.rotation);
        currentSegment.gameObject.transform.SetParent(transform);
        armParts.Add(currentSegment);
        currentSegment.distanceFromPlayer = (currentSegment.transform.position - gameObject.transform.position).magnitude;
    }

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