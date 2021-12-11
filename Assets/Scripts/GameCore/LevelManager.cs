using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
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
        DOTween.SetTweensCapacity(1000, 10);
        
        HexagonPooler.Instance.PrepareHexagonPools();
        BoardCreator.Instance.CreateBoard();
        BoardManager.Instance.CheckInitialHexagons();
    }


    private void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= OnGameSceneLoaded;
    }
}
