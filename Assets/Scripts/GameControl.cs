﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {
	public GameObject BacteriaGood;
	public GameObject BacteriaBad;
	public GameObject Antibiotic;
	public Vector3 worldSize; //how big is the world x, y, z
	public int numOfBacteria; //total number of bad and good bacteria to start.
	public int ratioDifficulty; //how many good bacteria spawn with every one bad bacteria
	public float waveWait;

	// Use this for initialization
	void Start () {
		worldSize = worldSize / 2;
		StartSpawnBacteriaGood (numOfBacteria / 2);
		StartBacteriaBad (numOfBacteria / 2 / ratioDifficulty);
		StartCoroutine (SpawnWave (ratioDifficulty));
	}
	void StartSpawnBacteriaGood (int maxHazard) {
		Quaternion spawnRotation = Quaternion.identity;

		for (int i = 0; i < maxHazard; i++) {
			Instantiate (BacteriaGood, 
				new Vector3 (Random.Range (-worldSize.x, worldSize.x), 
					Random.Range (-worldSize.y, worldSize.y),
					Random.Range (-worldSize.z, worldSize.z)),
				spawnRotation
			);

		}
	}
	void StartBacteriaBad(int maxHazard) {
		Quaternion spawnRotation = Quaternion.identity;

		for (int i = 0; i < maxHazard; i++) {
			Instantiate (BacteriaBad, 
				new Vector3 (Random.Range (-worldSize.x, worldSize.x), 
					Random.Range (-worldSize.y, worldSize.y),
					Random.Range (-worldSize.z, worldSize.z)),
				spawnRotation
			);

		}
	}
	IEnumerator SpawnWave(int ratio) {
		Quaternion spawnRotation = Quaternion.identity;
		int chanceOfAntibiotic = 30; //out of 100. The percentage of spawning antibiotic
		float dice;
		while (true) {
			//spawn one bad bacteria
			Instantiate (BacteriaBad, 
				new Vector3 (Random.Range (-worldSize.x, worldSize.x), 
					Random.Range (-worldSize.y, worldSize.y),
					Random.Range (-worldSize.z, worldSize.z)),
				spawnRotation
			);

			//spawn "ratio" number of good bacteria 
			for(int i = 0; i < ratio; i++){
					Instantiate (BacteriaGood, 
						new Vector3 (Random.Range (-worldSize.x, worldSize.x), 
							Random.Range (-worldSize.y, worldSize.y),
							Random.Range (-worldSize.z, worldSize.z)),
						spawnRotation
					);
				}
			yield return new WaitForSeconds (waveWait);
			dice = Random.Range (-chanceOfAntibiotic, 100 - chanceOfAntibiotic);
			if (dice > 0) {
				Instantiate (Antibiotic,  
					new Vector3 (Random.Range (-worldSize.x, worldSize.x), 
						Random.Range (-worldSize.y, worldSize.y),
						Random.Range (-worldSize.z, worldSize.z)),
					spawnRotation
				);
			}

		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
