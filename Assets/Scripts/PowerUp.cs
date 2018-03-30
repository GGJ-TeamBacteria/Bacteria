using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public enum PowerUpType { Extend, Super, Health};

    // Attribute value varies based on Type
    // Extend: --
    // Super: Duration of PowerUp
    // Health: HP increase
    public float m_Attribute; 

    public PowerUpType m_type;

	// Use this for initialization
	void Start () {
		if (m_type == PowerUpType.Extend)
        {
            m_Attribute = 0;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float GetAttribute()
    {
        return m_Attribute;
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
