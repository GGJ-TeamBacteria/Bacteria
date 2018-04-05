using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartObject : MonoBehaviour {

    public GameObject SFX_Appear;
    public GameObject SFX_Disappear;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<TentacleSegment>() != null)
        {
            Destroy(gameObject);
            GameManager.instance.StartGame();
        }
    }

    public void Activate()
    {
        gameObject.SetActive(true);
        if (SFX_Appear != null)
            Instantiate(SFX_Appear, transform.position, Quaternion.identity);
    }

    public void Deactivated()
    {
        gameObject.SetActive(false);
        if (SFX_Disappear != null)
            Instantiate(SFX_Disappear, transform.position, Quaternion.identity);
    }
}
