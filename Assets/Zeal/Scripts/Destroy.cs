using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour {
	public AudioClip audio0;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		//audioSource.GetComponent<AudioSource> ();
	}

	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other) {
		if (other.tag == "BacteriaGood") {
			Destroy (other.gameObject);
			GetComponent<AudioSource> ().Play ();

		}
	}
}
