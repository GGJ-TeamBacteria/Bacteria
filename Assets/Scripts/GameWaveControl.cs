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

    private static float SECONDS_PER_WAVE = 30;
    private static int NUMBER_OF_BATERIAS = 11;
    private static int NUMBER_OF_WAVE = 10;
    public Vector3 worldSize; //how big is the world x, y, z
    private int worldDifference = 5;
    private Vector3 worldSizeOuter;

    private GameObject[] listOfBacterias;

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
        int spawnWait = 3;
        int waveWait = 5;
        //yield return new WaitForSeconds(waveWait);
        float startTime;
        float currentTime;

        for (int level = 1; level < NUMBER_OF_WAVE; level++)
        {
            //start of each wave
            startTime = Time.time - 2.0f;
            currentTime = Time.time;
            Debug.Log("Wave Start");
            while (currentTime - startTime < SECONDS_PER_WAVE)
            {
                //for (int i = 0; i < 30; i++)
                //{
                    spawnObject(listOfBacterias[Random.Range(0, level)]); ;
                    yield return new WaitForSeconds(spawnWait);
                //}
                
                currentTime = Time.time;
            }
            yield return new WaitForSeconds(waveWait);
            Debug.Log("Wave Ended");
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
