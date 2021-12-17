using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreParametersScriptableObject scoreParameters;
    
    private float score;
    private bool active;
    private int previouslySpawnedBombCounter;
    
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
        if (score < (previouslySpawnedBombCounter + 1) * scoreParameters.BombHexagonSpawnScore) {return false;}

        previouslySpawnedBombCounter++;
        
        return true;
    }
    
    private void OnDisable()
    {
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
    }

    public float Score => score;
}
