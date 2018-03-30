using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public enum PowerUpType { Extend, Super, Health};

    public float m_Duration;
    public PowerUpType m_type;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetDuration()
    {
        return m_Duration;
    }

    public PowerUpType GetPowerUpType()
    {
        return m_type;
    }

    public void SetPowerUpType(PowerUpType type)
    {
        m_type = type;
    }
}
