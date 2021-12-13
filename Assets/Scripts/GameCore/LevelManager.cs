using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelStartParametersScriptableObject levelStartParameters;
 
    [SerializeField] private GameObject inputHandler;
    
    private void Awake()
    {
        GameManager.OnGameSceneLoaded += OnGameSceneLoaded;
        InitialHexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;
        
        inputHandler.gameObject.SetActive(false);
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
        InitialHexagonalGroupChecker.Instance.CheckInitialHexagons();
    }

    private void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= OnGameSceneLoaded;
        InitialHexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
    }
}
