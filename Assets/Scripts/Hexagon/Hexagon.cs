using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private BoardOperator boardOperator;
    
    private List<int[]> hexagonalGroup;
    private List<int[]> touchingIndexes;

    private Transform myTransform;

    private void OnEnable()
    {
        HexagonalGroupChecker.OnHexagonCleared += OnHexagonCleared;
        
        Initialize();
    }

    private void OnHexagonCleared(int i, int j)
    {
        if (!Active || HasBeenJustSpawned) {return;}
        if (i != IndexI || j >= IndexJ) {return;}
        
        DOTween.Kill(myTransform);
        CurrentTargetIndexJ--;
        //CurrentTargetHeight -= BoardCreator.Instance.HeightDifferenceBetweenHexagons;

        if (CurrentTargetIndexJ < 0 || CurrentTargetIndexJ > boardParameters.RowCount - 1) Debug.Log(i + " " + CurrentTargetIndexJ);

        CurrentTargetHeight = BoardCreator.Instance.GetHexagonYPosition(i, CurrentTargetIndexJ);
        
        //if (CurrentTargetIndexJ != CurrentTargetHeight2) {Debug.Log("that: " + i + " " +j);}
        
        myTransform.DOMoveY(CurrentTargetHeight, boardParameters.OldHexagonFallingDuration).OnComplete(() =>
        {
            boardOperator.RemoveHexagonFromBoardHexagonsList(this, IndexI, IndexJ);
            IndexJ = CurrentTargetIndexJ;
            boardOperator.AddHexagonToBoardHexagonsList(this, IndexI, IndexJ);
        });
        
    }

    public void Initialize()
    {
        Active = true;
        myTransform = transform;
        CurrentTargetHeight = myTransform.position.y;
        CurrentTargetIndexJ = IndexJ;
        SetTouchingHexagonIndexes();
    }

    private void SetTouchingHexagonIndexes()
    {
        touchingIndexes = new List<int[]>();

        touchingIndexes.Add(new int[] { IndexI - 1, IndexJ + (IndexI + 1) % 2 }); // On top left
        touchingIndexes.Add(new int[] { IndexI - 1, IndexJ - 1 + (IndexI + 1) % 2 }); // On bottom left
        touchingIndexes.Add(new int[] { IndexI, IndexJ - 1 }); // On bottom
        touchingIndexes.Add(new int[] { IndexI + 1, IndexJ - 1 + (IndexI + 1) % 2 }); // On bottom right
        touchingIndexes.Add(new int[] { IndexI + 1, IndexJ + (IndexI + 1) % 2 }); // On top right   
        touchingIndexes.Add(new int[] { IndexI, IndexJ + 1 }); // On top
    }

    public bool CheckHexagonalGroup(bool initialCheck)
    {
        if (!initialCheck) {SetTouchingHexagonIndexes();}
        
        HaveHexagonalGroup = false;
        hexagonalGroup = new List<int[]>();

        var previousHexagonWasOfMyColor = false;
        
        for (var i = 0; i < touchingIndexes.Count; i++)
        {
            if (color != boardOperator.GetHexagonColorOnIndex(touchingIndexes[i]))
            {
                previousHexagonWasOfMyColor = false;
                continue;
            }

            if (previousHexagonWasOfMyColor)
            {
                hexagonalGroup.Add(touchingIndexes[i]);
                hexagonalGroup.Add(touchingIndexes[i-1]);
                HaveHexagonalGroup = true;
                continue;
            }
            
            previousHexagonWasOfMyColor = true;
        }

        if (!previousHexagonWasOfMyColor || color != boardOperator.GetHexagonColorOnIndex(touchingIndexes[0])) { return HaveHexagonalGroup; }
        
        hexagonalGroup.Add(touchingIndexes[touchingIndexes.Count - 1]); 
        hexagonalGroup.Add(touchingIndexes[0]);
        HaveHexagonalGroup = true;

        return HaveHexagonalGroup;
    }
    
    private void OnDisable()
    {
        myTransform = null;

        HexagonalGroupChecker.OnHexagonCleared -= OnHexagonCleared;
    }

    public bool Chosen { get; set; }
    public bool HasBeenJustSpawned { get; set; }
    public int CurrentTargetIndexJ { get; set; }
    public float CurrentTargetHeight { get; set; }
    public bool Active { get; set; }
    public Transform MyTransform => myTransform;
    public bool HaveHexagonalGroup { get; set; }
    public List<int[]> HexagonalGroup => hexagonalGroup;
    public int IndexI { get; set; }
    public int IndexJ { get; set; }
    public Color color { get; set; }
}
