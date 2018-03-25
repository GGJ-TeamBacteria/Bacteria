using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Win : MonoBehaviour {


	void OnDestroy() {
		SceneManager.LoadScene("Win", LoadSceneMode.Single);
	}
}
