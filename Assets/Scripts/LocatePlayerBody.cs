using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocatePlayerBody : MonoBehaviour {

    public GameObject cameraHead;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        transform.position = cameraHead.transform.position;

        // Torso is below head
        // transform.Translate(Vector3.down * 0.15f, Space.World);
    }

    //void OnDrawGizmos()
    //{
    //    // Draw the player hitbox so the player understands it
    //    // Radius here must match the SphereCollider radius set on the PlayerBody object in the Unity inspector UI
    //    // (if scale wasn't = 1 on the PlayerBody.Transform we'd also need to take scale into account)
    //    Gizmos.color = Color.white;
    //    Gizmos.DrawWireSphere(transform.position, 0.5f);

    //    // Using a gizmo to display something for the player is a total hack which will 
    //    // only work when running the game under the Unity editor with the Gizmos option
    //    // turned on in the Game view settings (to the right of Maximize On Play) 

    //}
}
