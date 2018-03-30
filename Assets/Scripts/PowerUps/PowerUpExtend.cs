using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpExtend : MonoBehaviour, PowerUp {

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AffectPlayer(Tentacle tentacle)
    {
        tentacle.ExtendMaxLength(1);
    }
}
