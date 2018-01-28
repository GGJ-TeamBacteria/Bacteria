using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaShootProjectil : MonoBehaviour {
	public int chanceOfFire; //out of 100. The percentage of shooting per wait time
	public float waveWait;
	public GameObject projectile;
	private float dice;
	void Start () {
		StartCoroutine (Shoot ());
	}

	IEnumerator Shoot () {
		while (true) {
			yield return new WaitForSeconds (waveWait);

			dice = Random.Range (-chanceOfFire, 100 - chanceOfFire);
			if (dice < 0) {
				Instantiate (projectile, transform.position, Quaternion.identity);
			}
		}
	}
}
