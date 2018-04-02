using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour {


	void OnDestroy() {
        GameObject.Find("Game Control").GetComponent<GameWaveControl>().StartGame();
        //SceneManager.LoadScene("Win", LoadSceneMode.Single);
	}
}
