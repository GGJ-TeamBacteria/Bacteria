using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardPlayer : MonoBehaviour {
	public int speed;
	private Vector3 playerPosition;
	public GameObject sound;

	// Use this for initialization
	void Start () {
        Debug.Log("start");

        if (sound != null)
        {
            Instantiate(sound, transform.position, Quaternion.identity);
        }

        Debug.Log("after sound check" + sound);

        GameObject playerObject = GameObject.FindWithTag ("MainCamera");
		Transform playerTransform;

        Debug.Log("got player" + playerObject);

        playerTransform = playerObject.GetComponent <Transform>();

        Debug.Log("got player xform" + playerTransform.ToString());

        playerPosition = playerTransform.position;

        Debug.Log("got player position");


        var heading = playerPosition - transform.position;

        Debug.Log("Our heading is " + heading.ToString());

        var distance = heading.magnitude;
        Debug.Log("Our distance is " + distance.ToString());

        var direction = heading / distance;
        Debug.Log("Our direction is " + direction.ToString());

        GetComponent<Rigidbody> ().velocity = new Vector3 (direction.x, direction.y, direction.z) * speed * Time.deltaTime;
	}


}
