using System;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static event Action<float> OnGameOver;
    public static event Action OnNoPotentialValidMovePresent;

    [SerializeField] private LevelStartParametersScriptableObject levelStartParameters;

    [SerializeField] private PotentialValidMoveChecker potentialValidMoveChecker;
    [SerializeField] private ScoreManager scoreManager;
    
    [SerializeField] private GameObject inputHandler;

    private bool active;
    
    private void Awake()
    {
        active = true;
        
        GameManager.OnGameSceneLoaded += OnGameSceneLoaded;
        HexagonalGroupFinder.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;
        Bomb.OnBombExploded += OnBombExploded;
        PotentialValidMoveChecker.OnNoPotentialValidMoveLeft += OnNoPotentialValidMoveLeft;
        
        inputHandler.gameObject.SetActive(false);
    }

    private void OnNoPotentialValidMoveLeft()
    {
        if (!active) {return;}

        active = false;
        
        // If the game has just started, and no valid move is possible, renew the board.
        if (scoreManager.Score == 0) {OnNoPotentialValidMovePresent?.Invoke(); return;}
        
        GameOver();
    }

    private void GameOver()
    {
        OnGameOver?.Invoke(scoreManager.Score);
        inputHandler.SetActive(false);
    }
    
    private void OnBombExploded()
    {
        if (!active) {return;}

        active = false;
        
        GameOver();
    }

    private void OnAllInitialHexagonalGroupsCleared()
    {
        potentialValidMoveChecker.CheckIfThereIsAnyPotentialValidMoveLeft();
        inputHandler.gameObject.SetActive(true);
    }

    private void OnGameSceneLoaded()
    {
        DOTween.SetTweensCapacity(levelStartParameters.TweenCapacity, levelStartParameters.SequencesCapacity);

        StartGame();
    }

    private void StartGame()
    {
        HexagonPooler.Instance.PrepareHexagonPools();
        BoardCreator.Instance.CreateBoard();
        HexagonalGroupFinder.Instance.CheckInitialHexagonGroups();
    }

    private void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= OnGameSceneLoaded;
        HexagonalGroupFinder.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
        Bomb.OnBombExploded -= OnBombExploded;
        PotentialValidMoveChecker.OnNoPotentialValidMoveLeft -= OnNoPotentialValidMoveLeft;
    }
}
