using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroy : MonoBehaviour {
	public GameObject audio1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnDestroy()
    {

    }

    public void Death()
    {
        Debug.Log("Bat hit player");
        Instantiate(audio1, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
