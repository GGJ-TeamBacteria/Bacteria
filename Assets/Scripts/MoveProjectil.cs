using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveProjectil : MonoBehaviour {
	public GameObject sound;
	public float speed;

	// Use this for initialization
	void Start () {
		Instantiate (sound, transform.position, Quaternion.identity);
		GameObject playerObject = GameObject.FindWithTag ("MainCamera");
		Transform playerTransform;
		playerTransform = playerObject.GetComponent <Transform>();
		Vector3 playerPosition = playerTransform.position;


		var heading = playerPosition - transform.position;
		var distance = heading.magnitude;
		var direction = heading / distance;

		GetComponent<Rigidbody> ().velocity = new Vector3 (direction.x, direction.y, direction.z) * speed * Time.deltaTime;
	}
}
