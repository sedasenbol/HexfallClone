using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    
    private List<int[]> hexagonalGroup;
    private List<int[]> touchingIndexes;

    private Transform myTransform;
    private Tween fallingTween;

    private void OnEnable()
    {
        BoardManager.OnHexagonCleared += OnHexagonCleared;
        
        Initialize();
    }

    private void OnHexagonCleared(int i, int j)
    {
        if (!Active || HasBeenJustSpawned) {return;}
        if (i != IndexI || j >= IndexJ) {return;}
        
        DOTween.Kill(myTransform);
        CurrentTargetHeight -= BoardCreator.Instance.HeightDifferenceBetweenHexagons;
        CurrentTargetIndexJ--;
        fallingTween = myTransform.DOMoveY(CurrentTargetHeight, boardParameters.HexagonFallingDuration).OnComplete(() =>
        {
            BoardManager.Instance.AddHexagonToBoardHexagonsList(this, IndexI, IndexJ);
            IndexJ = CurrentTargetIndexJ;
        });
    }

    public void Initialize()
    {
        Active = true;
        myTransform = transform;
        CurrentTargetHeight = myTransform.position.y;
        CurrentTargetIndexJ = IndexJ;
        InitializeTouchingIndexes();
    }

    private void InitializeTouchingIndexes()
    {
        touchingIndexes = new List<int[]>();

        touchingIndexes.Add(new int[] { IndexI - 1, IndexJ + (IndexI + 1) % 2 }); // On top left
        touchingIndexes.Add(new int[] { IndexI - 1, IndexJ - 1 + (IndexI + 1) % 2 }); // On bottom left
        touchingIndexes.Add(new int[] { IndexI, IndexJ - 1 }); // On bottom
        touchingIndexes.Add(new int[] { IndexI + 1, IndexJ - 1 + (IndexI + 1) % 2 }); // On bottom right
        touchingIndexes.Add(new int[] { IndexI + 1, IndexJ + (IndexI + 1) % 2 }); // On top right   
        touchingIndexes.Add(new int[] { IndexI, IndexJ + 1 }); // On top
    }

    public bool CheckHexagonalGroup()
    {
        haveHexagonalGroup = false;
        hexagonalGroup = new List<int[]>();

        var previousHexagonWasOfMyColor = false;
        
        for (var i = 0; i < touchingIndexes.Count; i++)
        {
            if (color != BoardManager.Instance.GetHexagonColorOnIndex(touchingIndexes[i]))
            {
                previousHexagonWasOfMyColor = false;
                continue;
            }

            if (previousHexagonWasOfMyColor)
            {
                hexagonalGroup.Add(touchingIndexes[i]);
                hexagonalGroup.Add(touchingIndexes[i-1]);
                haveHexagonalGroup = true;
                continue;
            }
            
            previousHexagonWasOfMyColor = true;
        }

        if (!previousHexagonWasOfMyColor || color != BoardManager.Instance.GetHexagonColorOnIndex(touchingIndexes[0])) { return haveHexagonalGroup; }
        
        hexagonalGroup.Add(touchingIndexes[touchingIndexes.Count - 1]); 
        hexagonalGroup.Add(touchingIndexes[0]);
        haveHexagonalGroup = true;

        return haveHexagonalGroup;
    }
    
    private void OnDisable()
    {
        myTransform = null;

        BoardManager.OnHexagonCleared -= OnHexagonCleared;
    }

    public bool HasBeenJustSpawned { get; set; }
    public int CurrentTargetIndexJ { get; set; }
    public float CurrentTargetHeight { get; set; }
    public bool Active { get; set; }
    public Transform MyTransform => myTransform;
    public bool haveHexagonalGroup { get; set; }
    public List<int[]> HexagonalGroup => hexagonalGroup;
    public int IndexI { get; set; }
    public int IndexJ { get; set; }
    public Color color { get; set; }
}
