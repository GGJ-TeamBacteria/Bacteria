using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {
	public GameObject BacteriaGood;
	public GameObject BacteriaBad;
	public Vector3 worldSize;
	public int numOfBacteriaGood;

	// Use this for initialization
	void Start () {
		SpawnBacteriaGood (numOfBacteriaGood);
		SpawnBacteriaBad (numOfBacteriaGood);
	}
	void SpawnBacteriaGood (int maxHazard) {
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
	void SpawnBacteriaBad(int maxHazard) {
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

	// Update is called once per frame
	void Update () {
		
	}
}
