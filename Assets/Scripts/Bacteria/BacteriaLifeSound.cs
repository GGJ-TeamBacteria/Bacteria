using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaLifeSound : MonoBehaviour {

    public List<GameObject> audioSources;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        PlaySound();
    }

    void PlaySound()
    {
        //// play sound only 1 percent
        //if (Random.Range(0, 100) < 1)
        //    return;

        //if (audioSources != null || audioSources.Count > 0)
        //    Instantiate(audioSources[Random.Range(0, audioSources.Count)], transform.position, Quaternion.identity);

    }
}
