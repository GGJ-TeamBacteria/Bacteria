using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStartObject : MonoBehaviour {

    public int level;
    public GameObject SFX_Appear;
    public GameObject SFX_Disappear;

    public void Start()
    {
        TextMesh levelText = gameObject.GetComponentInChildren<TextMesh>();
        levelText.text = "Level " + level;
        transform.LookAt(GameManager.instance.playerHead.transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("kitakita");
        if (other.GetComponent<TentacleSegment>() != null)
        {
            GameManager.instance.StartGame(level);
        }
    }

}
