﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeTime : MonoBehaviour {

    public float maxLifeTime;
    private float lifeTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        lifeTime += Time.deltaTime;
        if (lifeTime >= maxLifeTime)
        {
            //Debug.Log(lifeTime);
            Destroy(gameObject);
        }
	}
}
