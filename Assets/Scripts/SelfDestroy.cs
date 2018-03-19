using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {
    public List<GameObject> audioSources;
    public GameObject destroyVFX;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    public void Death()
    {
        if (audioSources != null && audioSources.Count > 0) 
            Instantiate(audioSources[Random.Range(0, audioSources.Count)], transform.position, Quaternion.identity);

        if (destroyVFX != null)
            Instantiate(destroyVFX, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
