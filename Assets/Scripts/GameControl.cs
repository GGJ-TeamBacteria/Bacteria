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
	public Vector3 worldSize; //how big is the world x, y, z
	public int numOfBacteria; //total number of bad and good bacteria to start.
	public int ratioDifficulty; //how many good bacteria spawn with every one bad bacteria
	public float waveWait;
	public int chanceOfAntibiotic; //out of 100. The percentage of spawning antibiotic
	private float dice;
    private Quaternion spawnRotation = Quaternion.identity;

    // Use this for initialization
    void Start () {
		worldSize = worldSize / 2;
		StartBacteriaGood (numOfBacteria / 2);
		StartBacteriaBad (numOfBacteria / 2 / ratioDifficulty);
		StartBacteriaShoot (new Vector3 (0, -5, 20));
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
	void StartBacteriaShoot(Vector3 position) {
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
		
		while (true) {
            //spawn one bad bacteria
            SpawnRandomBadBacteria();

            //spawn "ratio" number of good bacteria 
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
		}
	}
}
