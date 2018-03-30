using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpHealth : MonoBehaviour, PowerUp
{

    // Attribute value varies based on Type
    // Extend: --
    // Super: Duration of PowerUp
    // Health: HP increase
    public float m_Health;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AffectPlayer(Tentacle tentacle)
    {
        tentacle.PowerUpHealth(m_Health);

    }
}
