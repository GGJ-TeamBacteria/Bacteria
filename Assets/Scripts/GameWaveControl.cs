using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWaveControl : MonoBehaviour {

    public GameObject BacteriaGood;
    public GameObject BacteriaBadBlue;
    public GameObject BacteriaBadOrange;
    public GameObject BacteriaBadPurple;

    private static int NUMBER_OF_WAVE = 10;
    public Vector3 worldSize; //how big is the world x, y, z



    // Use this for initialization
    void Start () {
        spawnObject(BacteriaGood);
        spawnObject(BacteriaGood);
        spawnObject(BacteriaGood);
        spawnObject(BacteriaGood);
    }

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
            case 'x': position = worldSize.x; break;
            case 'y': position = worldSize.y; break;
            case 'z': position = worldSize.z; break;
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
