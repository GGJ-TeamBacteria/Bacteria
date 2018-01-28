using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour {
	public GameObject BacteriaGood;
	public GameObject BacteriaBadBlue;
    public GameObject BacteriaBadOrange;
    public GameObject BacteriaBadPurple;
    public GameObject Antibiotic;
	public GameObject BacteriaShoot;
    public GameObject Heart;
	public Vector3 worldSize; //how big is the world x, y, z
	public int numOfBacteria; //total number of bad and good bacteria to start.
	public int ratioDifficulty; //how many good bacteria spawn with every one bad bacteria
	public float waveWait;
	public int chanceOfAntibiotic; //out of 100. The percentage of spawning antibiotic
	private float dice;
	public int numOfShooter; //cap at max of 5 shooters
    private Quaternion spawnRotation = Quaternion.identity;

    // Use this for initialization
    void Start () {
		worldSize = worldSize / 2;
		StartBacteriaGood (numOfBacteria / 2);
		StartBacteriaBad (numOfBacteria / 2 / ratioDifficulty);
		StartBacteriaShoot ();
		StartCoroutine (SpawnWave (ratioDifficulty));
	}
	void StartBacteriaGood (int maxHazard)
    {
		for (int i = 0; i < maxHazard; i++)
        {
            SpawnNewBacteria(BacteriaGood);
		}
	}
	void StartBacteriaBad(int maxHazard) {

		for (int i = 0; i < maxHazard; i++)
        {
            SpawnRandomBadBacteria();
		}
	}
	void StartBacteriaShoot() {
		Vector3 position = new Vector3 (0, -5, 20);
		if (numOfShooter > 5) //only allow up to 5 shooters
			numOfShooter = 5;
		float firstPosition = worldSize.x / 4; //calculate 5 locations along x axis for shooter
		for (int i = 0; i < numOfShooter; i++) {
			switch(i){
			case 0:
				Instantiate (BacteriaShoot, new Vector3 (firstPosition * 0, position.y, position.z), Quaternion.identity);
				break;
			case 1:
				Instantiate (BacteriaShoot, new Vector3 (firstPosition * 1, position.y, position.z), Quaternion.identity);
				break;
			case 2:
				Instantiate (BacteriaShoot, new Vector3 (firstPosition * -1, position.y, position.z), Quaternion.identity);
				break;
			case 3:
				Instantiate (BacteriaShoot, new Vector3 (firstPosition * 2, position.y, position.z), Quaternion.identity);
				break;
			case 4:
				Instantiate (BacteriaShoot, new Vector3 (firstPosition * -2, position.y, position.z), Quaternion.identity);
				break;
			}
		}
		Instantiate (BacteriaShoot, position, Quaternion.identity);
	}

    void SpawnNewBacteria(GameObject bacteria)
    {
        Instantiate(bacteria,
                new Vector3(Random.Range(-worldSize.x, worldSize.x),
                    Random.Range(-worldSize.y, worldSize.y),
                    Random.Range(-worldSize.z, worldSize.z)),
                spawnRotation
            );
    }

    void SpawnRandomBadBacteria()
    {
        switch (Random.Range(1, 4))
        {
            case 1:
                SpawnNewBacteria(BacteriaBadBlue);
                break;
            case 2:
                SpawnNewBacteria(BacteriaBadOrange);
                break;
            case 3:
                SpawnNewBacteria(BacteriaBadPurple);
                break;
        }
    }

	IEnumerator SpawnWave(int ratio) {
		Quaternion spawnRotation = Quaternion.identity;

		float antibiotic_dice;
        //float heart_timer = 2f;
		
		while (true) {
            //spawn one bad bacteria
            SpawnRandomBadBacteria();

            //spawn "ratio" number of good bacteria 

            for (int i = 0; i < ratio; i++) {
				SpawnNewBacteria (BacteriaGood);
			}
			yield return new WaitForSeconds (waveWait);

			antibiotic_dice = Random.Range (-chanceOfAntibiotic, 100 - chanceOfAntibiotic);
			if (antibiotic_dice < 0) {
				SpawnNewBacteria (Antibiotic);
			}

            for (int i = 0; i < ratio; i++)
            {
                SpawnNewBacteria(BacteriaGood);
            }
			yield return new WaitForSeconds (waveWait);

			antibiotic_dice = Random.Range (-chanceOfAntibiotic, 100 - chanceOfAntibiotic);
			if (antibiotic_dice < 0)
            {
                SpawnNewBacteria(Antibiotic);
            }
            /*
            heart_timer -= Time.deltaTime;
            if (heart_timer < 0)
            {
                SpawnNewBacteria(Heart);
            }
            */

        }
	}
}
