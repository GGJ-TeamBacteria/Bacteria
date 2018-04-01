using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BacteriaShootProjectil : MonoBehaviour {
	public int chanceOfFire; //out of 100. The percentage of shooting per wait time
	public float waveWait;
	public GameObject projectile;
    public Transform projectileSpawn;
	private float dice;
   
	void Start () {
		StartCoroutine (Shoot ());
	}
	public void setFireRate(int chance) {
		chanceOfFire = chance;
	}
	IEnumerator Shoot () {
		while (true) {
			yield return new WaitForSeconds (waveWait);

			dice = Random.Range (-chanceOfFire, 100 - chanceOfFire);
			if (dice < 0) {
                Fire();
			}
		}
	}
    public void Fire()
    {
        Instantiate(projectile, projectileSpawn.position, Quaternion.identity);
    }
}
