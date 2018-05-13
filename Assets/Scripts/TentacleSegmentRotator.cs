using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleSegmentRotator : MonoBehaviour {

    [Tooltip("Angular velocity in degrees per seconds")]
    public float degPerSec = 60.0f;

    [Tooltip("Rotation axis")]
    public Vector3 rotAxis = Vector3.up;

    // Use this for initialization
    private void Start()
    {
        degPerSec = Random.Range(20, 80);

        float foo = Random.Range(0, 10);
        if (foo % 3 == 0)
        {
            rotAxis = new Vector3(0, Random.Range(0, 360), 0);
        }

        rotAxis.Normalize();
    }

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(rotAxis, degPerSec * Time.deltaTime);
    }
}
