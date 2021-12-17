using System;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static event Action<float> OnGameOver;
    public static event Action OnNoPotentialValidMovePresent;

    [SerializeField] private LevelStartParametersScriptableObject levelStartParameters;
    [SerializeField] private HexagonalGroupChecker hexagonalGroupChecker;
    [SerializeField] private PotentialValidMoveChecker potentialValidMoveChecker;

    [SerializeField] private GameObject inputHandler;
    [SerializeField] private ScoreManager scoreManager;

    private bool active;
    
    private void Awake()
    {
        active = true;
        
        GameManager.OnGameSceneLoaded += OnGameSceneLoaded;
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;
        Bomb.OnBombExploded += OnBombExploded;
        PotentialValidMoveChecker.OnNoPotentialMoveLeft += OnNoPotentialMoveLeft;
        
        inputHandler.gameObject.SetActive(false);
    }

    private void OnNoPotentialMoveLeft()
    {
        if (!active) {return;}

        active = false;
        
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
        DOTween.SetTweensCapacity(levelStartParameters.TweenersCapacity, levelStartParameters.SequencesCapacity);

        StartGame();
    }

    private void StartGame()
    {
        HexagonPooler.Instance.PrepareHexagonPools();
        BoardCreator.Instance.CreateBoard();
        hexagonalGroupChecker.CheckInitialHexagonGroups();
    }

    private void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= OnGameSceneLoaded;
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
        Bomb.OnBombExploded -= OnBombExploded;
        PotentialValidMoveChecker.OnNoPotentialMoveLeft -= OnNoPotentialMoveLeft;
    }
}
