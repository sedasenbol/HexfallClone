using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static event Action OnGameSceneLoaded;
    
    private GameState gameState = new GameState();

    private void OnEnable()
    {
        UIManager.OnPlayButtonClicked += StartGame;
        UIManager.OnRestartButtonClicked += RestartGame;
        UIManager.OnPauseButtonClicked += PauseGame;
        UIManager.OnResumeButtonClicked += ResumeGame;
        UIManager.OnHomePageButtonClicked += LoadHomePage;
        
        SceneManager.sceneLoaded += OnSceneLoaded;
        LevelManager.OnGameOver += OnGameOver;
        LevelManager.OnNoPotentialValidMovePresent += OnNoPotentialValidMovePresent;
    }

    private void OnNoPotentialValidMovePresent()
    {
        RestartGame();
    }

    private void OnGameOver(float score)
    {
        gameState.CurrentState = GameState.State.Over;
        UIManager.Instance.LoadGameOverScreen(score);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene != SceneManager.GetSceneByBuildIndex((int) GameState.Scene.Game)) {return;}

        SceneManager.SetActiveScene(scene);
        OnGameSceneLoaded?.Invoke();
    }

    private void StartGame()
    {
        SceneManager.LoadScene((int) GameState.Scene.Game, LoadSceneMode.Additive);
        
        gameState.CurrentScene = GameState.Scene.Game;
        gameState.CurrentState = GameState.State.Play;
    }

    private void RestartGame()
    {
        DOTween.KillAll();
        SceneManager.UnloadSceneAsync((int) (GameState.Scene.Game));
        gameState = new GameState();
        
        StartGame();
    }
    
    private void PauseGame()
    {
        Time.timeScale = 0f;
        gameState.CurrentState = GameState.State.Paused;
    }

    private void ResumeGame()
    {
        Time.timeScale = 1f;
        gameState.CurrentState = GameState.State.Play;
    }

    private void LoadHomePage()
    {
        SceneManager.UnloadSceneAsync((int) (GameState.Scene.Game));
        Time.timeScale = 1f;

        gameState = new GameState(); 
    }

    private void OnDisable()
    {
        UIManager.OnPlayButtonClicked -= StartGame;
        UIManager.OnRestartButtonClicked -= RestartGame;
        UIManager.OnPauseButtonClicked -= PauseGame;
        UIManager.OnResumeButtonClicked -= ResumeGame;
        UIManager.OnHomePageButtonClicked -= LoadHomePage;
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        LevelManager.OnGameOver -= OnGameOver;
        LevelManager.OnNoPotentialValidMovePresent -= OnNoPotentialValidMovePresent;
    }
}
