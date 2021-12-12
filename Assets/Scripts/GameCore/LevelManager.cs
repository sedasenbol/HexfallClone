using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelStartParametersScriptableObject levelStartParameters; 
    
    private void Awake()
    {
        GameManager.OnGameSceneLoaded += OnGameSceneLoaded;
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
        StartCoroutine(CheckInitialHexagonsWithDelay());
    }

    private IEnumerator CheckInitialHexagonsWithDelay()
    {
        yield return new WaitForSeconds(levelStartParameters.InitialHexagonCheckDelay);
        
        BoardManager.Instance.CheckInitialHexagons();
    }

    private void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= OnGameSceneLoaded;
    }
}
