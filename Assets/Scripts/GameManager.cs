﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager> {

    enum GameState { Menu, InGame, Result, Pause };
    enum InGameState { Travelling, InLevel };

    public override void SingletonAwake() { }

    public PlayerScript player;
    public int levelNumber;
    public GameWaveControl gameWaveControl;
    public GameStartObject gameStartObject;
    public GameObject playerHead;
    public float secForSpawningGameStartObj = 10.0f;
    public GameObject menu;

    private GameState currentGameState;
    private int playerProgress;

    public void Start()
    {
        BGMManager.instance.Play(BGM.Idle);
        currentGameState = GameState.Menu;
    }

    public void StartGame(int level)
    {
        // prevent a game starts again and again
        if (currentGameState == GameState.InGame)
            return;

        currentGameState = GameState.InGame;
        player.Initialize();
        gameWaveControl.StartGame(level);
        menu.SetActive(false);

        BGMManager.instance.Play(BGM.Gameplay);
    }

    public void WinGame()
    {
        currentGameState = GameState.Result;
        gameWaveControl.stopWave();
        BGMManager.instance.Play(BGM.Win);

        StartCoroutine("InitializeGame");
    }

    public void LostGame()
    {
        currentGameState = GameState.Result;
        gameWaveControl.stopWave();
        BGMManager.instance.Play(BGM.Lose);

        StartCoroutine("InitializeGame");
    }

    IEnumerator InitializeGame()
    {
        yield return new WaitForSeconds(secForSpawningGameStartObj);

        if (currentGameState == GameState.Result)
        {
            currentGameState = GameState.Menu;
            player.Initialize();
            menu.SetActive(true);
        }

    }
}
