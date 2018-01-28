using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {
    public List<GameObject> audioSources;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Player")
    //    {
    //        Debug.Log("Bat hit player");
    //        Instantiate(audio1, transform.position, Quaternion.identity);
    //        Destroy(gameObject);
    //    }
    //}

    public void Death()
    {
        Debug.Log("Bat hit player");
        if (audioSources != null || audioSources.Count > 0) 
            Instantiate(audioSources[Random.Range(0, audioSources.Count)], transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
