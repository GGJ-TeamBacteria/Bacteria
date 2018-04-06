using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    public override void SingletonAwake() { }

    public PlayerScript player;
    public GameWaveControl gameWaveControl;
    public GameStartObject gameStartObject;
    public GameObject playerHead;

    public void Start()
    {
        BGMManager.instance.Play(BGM.Idle);
        gameWaveControl.HelpPlayerToStart();
    }

    public void StartGame()
    {
        player.Initialize();
        gameWaveControl.StopHelpingPlayerToStart();
        gameWaveControl.StartGame();
        gameStartObject.Deactivated();

        BGMManager.instance.Play(BGM.Gameplay);
    }

    public void WinGame()
    {
        gameWaveControl.stopWave();
        BGMManager.instance.Play(BGM.Win);

        //StartCoroutine("InitializeGame");
    }

    public void LostGame()
    {
        gameWaveControl.stopWave();
        BGMManager.instance.Play(BGM.Lose);

        //StartCoroutine("InitializeGame");
    }

    IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(10.0f);

        player.Initialize();
        gameStartObject.Activate();
        //gameWaveControl.HelpPlayerToStart();
    }
}
