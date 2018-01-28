﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {
	public GameObject BacteriaGood;
	public GameObject BacteriaBadBlue;
    public GameObject BacteriaBadOrange;
    public GameObject BacteriaBadPurple;
    public GameObject Antibiotic;
	public Vector3 worldSize; //how big is the world x, y, z
	public int numOfBacteria; //total number of bad and good bacteria to start.
	public int ratioDifficulty; //how many good bacteria spawn with every one bad bacteria
	public float waveWait;
	public int chanceOfAntibiotic; //out of 100. The percentage of spawning antibiotic

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

		float antibiotic_dice;
        int bacteriaBad_dice;
        while (true) {
            //spawn one bad bacteria
            bacteriaBad_dice = Random.Range(1, 3);

            switch (bacteriaBad_dice)
            {
                case 1:
                    Instantiate(BacteriaBadBlue,
                        new Vector3(Random.Range(-worldSize.x, worldSize.x),
                        Random.Range(-worldSize.y, worldSize.y),
                        Random.Range(-worldSize.z, worldSize.z)),
                        spawnRotation
                        );
                    break;
                case 2:
                    Instantiate(BacteriaBadOrange,
                        new Vector3(Random.Range(-worldSize.x, worldSize.x),
                        Random.Range(-worldSize.y, worldSize.y),
                        Random.Range(-worldSize.z, worldSize.z)),
                        spawnRotation
                        );
                    break;
                case 3:
                    Instantiate(BacteriaBadPurple,
                        new Vector3(Random.Range(-worldSize.x, worldSize.x),
                        Random.Range(-worldSize.y, worldSize.y),
                        Random.Range(-worldSize.z, worldSize.z)),
                        spawnRotation
                        );
                    break;
        }

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

            antibiotic_dice = Random.Range (-chanceOfAntibiotic, 100 - chanceOfAntibiotic);
			if (antibiotic_dice < 0) {
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
