using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpSuper : MonoBehaviour, PowerUp
{

    // Attribute value varies based on Type
    // Extend: --
    // Super: Duration of PowerUp
    // Health: HP increase
    public float m_Duration;

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
        tentacle.PowerUpSuper(m_Duration);

    }
}
