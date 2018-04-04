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

    public PowerUpSuper PowerUpSuper;
    public GameObject PowerUpHealth;
    public GameObject PowerUpExtend;

    public float playerReachRadius;

    private static float SECONDS_PER_WAVE = 30;
    private static int NUMBER_OF_BATERIAS = 11;
    private static int NUMBER_OF_WAVE = 20;
    public Vector3 worldSize; //how big is the world x, y, z
    private int worldDifference = 5;
    private Vector3 worldSizeOuter;

    private GameObject[] listOfBacterias;

    private int gameOrigin = 0;

    public TextMesh waveStatusReadout;
    //public Color normalColor;
    public Color attenColor;

    // Use this for initialization
    void Start() {

        worldSizeOuter = worldSize + new Vector3(worldDifference, worldDifference, worldDifference);
        addBatToList();
        attenColor = Color.red;
    }

    //Call this to start the game
    public void StartGame()
    {
        StartCoroutine("spawnWave");
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
        int waveWait = 10;
        Color normalcolor = waveStatusReadout.color;

        float startTime;
        float currentTime;
        //count down to wave
        waveStatusReadout.color = attenColor;
        for (int i = waveWait; i > 0; i--)
        {
            yield return new WaitForSeconds(1);
            waveStatusReadout.text = "Starting in " + i;
        }
        waveStatusReadout.color = normalcolor;

        int spawnCounter = 0;
        for (int level = 1; level <= NUMBER_OF_WAVE; level++)
        {
            waveStatusReadout.text = "WAVE: " + level + "/" + NUMBER_OF_WAVE;
            //shooter spawn at every 3th level
            if (level % 3 == 0)
            {
                Instantiate(BacteriaShooter, new Vector3(0, 0, 40), Quaternion.identity); //make it variables 
                GameObject shooter = GameObject.FindWithTag("Shooter");
                shooter.GetComponent<BacteriaShootProjectil>().setFireRate(90); //make it variables 
                yield return new WaitForSeconds(5);
                shooter.GetComponent<BacteriaShootProjectil>().setFireRate(9); //make it variables 
            }
            //spawn temp extend pill
            if (level == 2)
            {
                // One for left hand and one for right hand
                //Instantiate(PowerUpExtend, new Vector3(-2.2f, 0f, 1.9f), Quaternion.identity);
                //Instantiate(PowerUpExtend, new Vector3(2.2f, 0f, 1.9f), Quaternion.identity);
            }
            //Spawn the length-extension power ups after the player has got experience with the original length
            if (level % 4 == 0)
            {
                // One for left hand and one for right hand
                Instantiate(PowerUpSuper, new Vector3(-2.2f, 0f, 1.9f), Quaternion.identity);
                Instantiate(PowerUpSuper, new Vector3(2.2f, 0f, 1.9f), Quaternion.identity);
            }
            //Start wave
            Debug.Log("Wave Start");
            startTime = Time.time - 2.0f;
            currentTime = Time.time;
            while (currentTime - startTime < SECONDS_PER_WAVE)
            {
                //for (int i = 0; i < 30; i++)
                //{
                spawnObject(listOfBacterias[Random.Range(0, level)]);
                spawnCounter++;

                yield return new WaitForSeconds(spawnWait);
                //}

                currentTime = Time.time;
            }
            //wait untill all bacterias are destroy
            while (GameObject.FindGameObjectWithTag("BadBacteria") != null)
            {
                yield return new WaitForSeconds(1);
            }
            Debug.Log("Wave Ended");
            yield return new WaitForSeconds(1); // buffer time
            //display next wave text
            waveStatusReadout.color = attenColor;
            for (int i = waveWait; i > 0; i--)
            {
                yield return new WaitForSeconds(1);
                waveStatusReadout.text = "next wave starting in " + i;
            }
            waveStatusReadout.color = normalcolor;
            Instantiate(PowerUpHealth, new Vector3(-0.3f, 0f, 1.9f), Quaternion.identity);

            // Each wave is harder
            spawnWait = spawnWait - 0.125f;
        }
        winGame();
    }

    //spawn gameObject randomly at the edage for worldSize cube
    void spawnObject(GameObject gameObject)
    {
        Instantiate(gameObject, randomLocation(), Quaternion.identity);
    }
    //return a random Vector3 location with in world
    Vector3 randomLocation()
    {
        Vector3 temp;
        if (Random.Range(0, 2) == 0)
        {
            temp = new Vector3(randomPos('x', true), randomPos('y', false), randomPos('z', false));
        }
        else
        {
            temp = new Vector3(randomPos('x', false), randomPos('y', true), randomPos('z', false));
        }
        return temp;
    }
    //return a positive worldSize or negative worldSize for the axis parameter
    float randomPos(char axis, bool restricted)
    {
        float position;
        if (restricted)
        {
            switch (axis)
            {
                case 'x': position = Random.Range(worldSize.x, worldSizeOuter.x); break;
                case 'y': position = Random.Range(worldSize.y, worldSizeOuter.y); break;
                case 'z': position = Random.Range(worldSize.z, worldSizeOuter.z); break;
                default: position = 0; break;
            }
        }
        else
        {
            switch (axis)
            {
                case 'x': position = Random.Range(gameOrigin, worldSizeOuter.x); break;
                case 'y': position = Random.Range(gameOrigin, worldSizeOuter.y); break;
                case 'z': position = Random.Range(gameOrigin, worldSizeOuter.z); break;
                default: position = 0; break;
            }
        }
        if (Random.Range(0, 2) == 0)
        {
            //restricted all bacterias to spawn from positive x direction only
            if (axis != 'x')
            {
                position = -position;
            }

        }
        return position;
    }
    // Update is called once per frame
    void Update() {

    }
    public void stopWave()
    {
        StopCoroutine("spawnWave");
    }
    public void winGame()
    {
        GameManager.instance.WinGame();
    }

    public void HelpPlayerToStart()
    {
        StartCoroutine("HelpPlayerToStartCoroutine");
    }

    public void StopHelpingPlayerToStart()
    {
        StopCoroutine("HelpPlayerToStartCoroutine");
    }

    IEnumerator HelpPlayerToStartCoroutine()
    {
        while (true)
        {
            Instantiate( PowerUpSuper, GetRandomPlaceAroundTarget(GameManager.instance.playerHead, playerReachRadius), Quaternion.identity);
            yield return new WaitForSeconds(PowerUpSuper.m_Duration + 1.0f);
        }
    }

    private Vector3 GetRandomPlaceAroundTarget(GameObject target, float radius)
    {
        return new Vector3(
                     target.transform.position.x + (-radius * Mathf.Cos(Random.Range(0.1f, 1.0f)) * Mathf.Cos(Random.Range(0.1f, 1.0f))),
                     target.transform.position.y + (radius * Mathf.Sin(Random.Range(0.1f, 1.0f))),
                     target.transform.position.z + radius * Mathf.Cos(Random.Range(0.1f, 1.0f)) * Mathf.Sin(Random.Range(0.1f, 1.0f))
                );
    }
}
