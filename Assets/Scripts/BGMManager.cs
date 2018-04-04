using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BGM
{
    Idle, Gameplay, Win, Lose
}

[RequireComponent(typeof(AudioSource))]
public class BGMManager : Singleton<BGMManager> {
    public override void SingletonAwake() { }

    public AudioClip MUS_Idle;
    public AudioClip MUS_Gameplay;
    public AudioClip MUS_Win;
    public AudioClip MUS_Lose;

    private AudioSource audioSource;

    public void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void Play(BGM bgmType)
    {

        audioSource.Stop();

        switch (bgmType)
        {
            case BGM.Idle:
                if (MUS_Idle == null)
                    return;

                audioSource.clip = MUS_Idle;
                break;

            case BGM.Gameplay:
                if (MUS_Gameplay == null)
                    return;

                audioSource.clip = MUS_Gameplay;
                break;

            case BGM.Win:
                if (MUS_Win == null)
                    return;

                audioSource.clip = MUS_Win;
                break;

            case BGM.Lose:
                if (MUS_Lose == null)
                    return;

                audioSource.clip = MUS_Lose;
                break;

        }

        audioSource.Play();
    }

}
