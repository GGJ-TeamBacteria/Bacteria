using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {
	public GameObject BacteriaGood;
	public GameObject BacteriaBad;
	public Vector3 worldSize; //how big is the world x, y, z
	public int numOfBacteria; //total number of bad and good bacteria to start.
	public int ratioDifficulty; //how many bad bacteria spawn with every one good bacteria

	// Use this for initialization
	void Start () {
		StartSpawnBacteriaGood (numOfBacteria / 2);
		StartBacteriaBad (numOfBacteria / 2);
		StartCoroutine (SpawnWave (ratioDifficulty));
	}
	void StartSpawnBacteriaGood (int maxHazard) {
		Quaternion spawnRotation = Quaternion.identity;

		for (int i = 0; i < maxHazard; i++) {
			Instantiate (BacteriaGood, 
				new Vector3 (Random.Range (-worldSize.x / 2, worldSize.x / 2), 
					Random.Range (-worldSize.y / 2, worldSize.y / 2),
					Random.Range (-worldSize.z / 2, worldSize.z / 2)),
				spawnRotation
			);

		}
	}
	void StartBacteriaBad(int maxHazard) {
		Quaternion spawnRotation = Quaternion.identity;

		for (int i = 0; i < maxHazard; i++) {
			Instantiate (BacteriaBad, 
				new Vector3 (Random.Range (-worldSize.x / 2, worldSize.x / 2), 
					Random.Range (-worldSize.y / 2, worldSize.y / 2),
					Random.Range (-worldSize.z / 2, worldSize.z / 2)),
				spawnRotation
			);

		}
	}
	IEnumerator SpawnWave(int ratio) {
		Quaternion spawnRotation = Quaternion.identity;
		while (true) {
			//spawn one good bacteria
			Instantiate (BacteriaGood, 
				new Vector3 (Random.Range (-worldSize.x / 2, worldSize.x / 2), 
					Random.Range (-worldSize.y, worldSize.y),
					Random.Range (-worldSize.z, worldSize.z)),
				spawnRotation
			);

			//spawn "ratio" number of bad bacteria 
			for(int i = 0; i < ratio; i++){
					Instantiate (BacteriaBad, 
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
