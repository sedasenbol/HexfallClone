using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreParametersScriptableObject scoreParameters;
    
    private float score;
    private bool active;
    
    private void OnEnable()
    {
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;
        HexagonalGroupChecker.OnHexagonCleared += OnHexagonCleared;
    }

    private void OnAllInitialHexagonalGroupsCleared()
    {
        active = true;
    }
    
    private void OnHexagonCleared(int i, int j)
    {
        if (!active) {return;}
        
        score += scoreParameters.ScorePerClearedHexagon;
        UIManager.Instance.UpdateScore(score);
    }
    
    private void OnDisable()
    {
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
        HexagonalGroupChecker.OnHexagonCleared -= OnHexagonCleared;
    }

    public float Score => score;
}
