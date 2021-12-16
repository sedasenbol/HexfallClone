using System;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static event Action<float> OnGameOver;
    
    [SerializeField] private LevelStartParametersScriptableObject levelStartParameters;
    [SerializeField] private HexagonalGroupChecker hexagonalGroupChecker;
    
    [SerializeField] private GameObject inputHandler;
    [SerializeField] private ScoreManager scoreManager;
    
    private void Awake()
    {
        GameManager.OnGameSceneLoaded += OnGameSceneLoaded;
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;
        Bomb.OnBombExploded += OnBombExploded; 
        
        inputHandler.gameObject.SetActive(false);
    }

    private void OnBombExploded()
    {
        OnGameOver?.Invoke(scoreManager.Score);
        inputHandler.SetActive(false);
    }

    private void OnAllInitialHexagonalGroupsCleared()
    {
        inputHandler.gameObject.SetActive(true);
    }

    private void OnGameSceneLoaded()
    {
        StartGame();
    }

    private void StartGame()
    {
        DOTween.SetTweensCapacity(levelStartParameters.TweenersCapacity, levelStartParameters.SequencesCapacity);
        
        HexagonPooler.Instance.PrepareHexagonPools();
        BoardCreator.Instance.CreateBoard();
        hexagonalGroupChecker.CheckInitialHexagonGroups();
    }

    private void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= OnGameSceneLoaded;
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
        Bomb.OnBombExploded -= OnBombExploded;
    }
}
