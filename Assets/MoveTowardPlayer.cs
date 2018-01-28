using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardPlayer : MonoBehaviour {
	public int speed;
	private Transform playerTransform;

	// Use this for initialization
	void Start () {
		GameObject playerObject = GameObject.FindWithTag ("Player");
		if (playerObject != null)
		{
			playerTransform = playerObject.GetComponent <Transform>();
		}
		if (playerObject == null)
		{
			Debug.Log ("Cannot find 'GameController' script");
		}
		float xMove = playerTransform.position.x - transform.position.x;
		float yMove = playerTransform.position.y - transform.position.y;
		float zMove = playerTransform.position.z - transform.position.z;

		Debug.Log (xMove);
		Debug.Log (yMove);
		Debug.Log (zMove);

		GetComponent<Rigidbody> ().velocity = new Vector3 (
			xMove * speed,
			yMove * speed,
			zMove * speed
		);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
