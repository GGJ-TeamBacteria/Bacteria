using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWaveControl : MonoBehaviour {


    public GameObject BacteriaSlowRed;
    public GameObject BacteriaSlowGreen;
    public GameObject BacteriaSlowBlue;
    public GameObject BacteriaSlowOrange;
    public GameObject BacteriaMidPurple;
    public GameObject BacteriaMidOrange;
    public GameObject BacteriaMidGreen;
    public GameObject BacteriaMidBlue;
    public GameObject BacteriaFastYellow;
    public GameObject BacteriaFastRed;
    public GameObject BacteriaFastPurple;
    public GameObject BacteriaShooter;

    public GameObject PowerUpSuper;
    public GameObject PowerUpHealth;

    private static float SECONDS_PER_WAVE = 30;
    private static int NUMBER_OF_BATERIAS = 11;
    private static int NUMBER_OF_WAVE = 10;
    public Vector3 worldSize; //how big is the world x, y, z
    private int worldDifference = 5;
    private Vector3 worldSizeOuter;

    private GameObject[] listOfBacterias;

    public TextMesh waveStatusReadout;

    // Use this for initialization
    void Start () {
        
        worldSizeOuter = worldSize + new Vector3(worldDifference, worldDifference, worldDifference);
        addBatToList();

        StartCoroutine (spawnWave());
        
    }
    void addBatToList()
    {
        listOfBacterias = new GameObject[NUMBER_OF_BATERIAS];
        listOfBacterias[0] = BacteriaSlowRed;
        listOfBacterias[1] = BacteriaSlowGreen;
        listOfBacterias[2] = BacteriaSlowBlue;
        listOfBacterias[3] = BacteriaSlowOrange;
        listOfBacterias[4] = BacteriaMidPurple;
        listOfBacterias[5] = BacteriaMidOrange;
        listOfBacterias[6] = BacteriaMidGreen;
        listOfBacterias[7] = BacteriaMidBlue;
        listOfBacterias[8] = BacteriaFastYellow;
        listOfBacterias[9] = BacteriaFastRed;
        listOfBacterias[10] = BacteriaFastPurple;
        
    }

    IEnumerator spawnWave()
    {
        float spawnWait = 2.5f;
        int waveWait = 5;
        //Instantiate(BacteriaShooter, new Vector3(0, 0, 10), Quaternion.identity);
        //yield return new WaitForSeconds(waveWait);
        float startTime;
        float currentTime;

        int spawnCounter = 0;
        for (int level = 1; level <= NUMBER_OF_WAVE; level++)
        {
            waveStatusReadout.text = "WAVE: " + level + "/" + NUMBER_OF_WAVE;

            //start of each wave
            startTime = Time.time - 2.0f;
            currentTime = Time.time;
            Debug.Log("Wave Start");
            while (currentTime - startTime < SECONDS_PER_WAVE)
            {
                //for (int i = 0; i < 30; i++)
                //{
                spawnObject(listOfBacterias[Random.Range(0, level)]); 
                spawnCounter++;

                // Spawn the length-extension power ups after the player has got some
                // experience with the original length
                if (spawnCounter == 8)
                {
                    // One for left hand and one for right hand
                    Instantiate(PowerUpSuper, new Vector3(-2.2f, 0f, 1.9f), Quaternion.identity);
                    Instantiate(PowerUpSuper, new Vector3(2.2f, 0f, 1.9f), Quaternion.identity);
                }

                yield return new WaitForSeconds(spawnWait);
                //}
                
                currentTime = Time.time;
            }
            yield return new WaitForSeconds(waveWait);
            Debug.Log("Wave Ended");
            Instantiate(PowerUpHealth, new Vector3(-0.3f, 0f, 1.9f), Quaternion.identity);

            // Each wave is harder
            spawnWait = spawnWait - 0.125f;   
        }
    }
    
    //spawn gameObject randomly at the edage for worldSize cube
    void spawnObject(GameObject gameObject)
    {
        Instantiate(gameObject, new Vector3(randomPos('x'), randomPos('y'), randomPos('z')), Quaternion.identity);
    }
    //return a positive worldSize or negative worldSize for the axis parameter
    float randomPos(char axis)
    {
        float position;
        switch (axis)
        {
            case 'x': position = Random.Range(worldSize.x, worldSizeOuter.x); break;
            case 'y': position = Random.Range(worldSize.y, worldSizeOuter.y); break;
            case 'z': position = Random.Range(worldSize.z, worldSizeOuter.z); break;
            default: position = 0; break;
        }
        
        if (Random.Range(0, 2) == 0)
        {
            position = -position;
        }
        return position;
    }
	// Update is called once per frame
	void Update () {
		
	}
}
