using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreParametersScriptableObject scoreParameters;
    
    private float score;
    private bool active;
    private int bombCounter = 1;
    
    private void OnEnable()
    {
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;
    }

    private void OnAllInitialHexagonalGroupsCleared()
    {
        active = true;
    }
    
    public void UpdateScore()
    {
        if (!active) {return;}
        
        score += scoreParameters.ScorePerClearedHexagon;
        UIManager.Instance.UpdateScore(score);
    }

    public bool ShouldSpawnBomb()
    {
        if (score < bombCounter * scoreParameters.BombHexagonSpawnScore) {return false;}

        bombCounter++;
        
        return true;
    }
    
    private void OnDisable()
    {
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
    }

    public float Score => score;
}
