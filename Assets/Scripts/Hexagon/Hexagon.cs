using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private BoardOperator boardOperator;
    
    private List<int[]> hexagonalGroup;
    private List<int[]> neighborHexagonIndexes;
    private int[] colorCountArray;

    private Transform myTransform;

    protected virtual void OnEnable()
    {
        HexagonalGroupChecker.OnHexagonCleared += OnHexagonCleared;
        
        hexagonalGroup = new List<int[]>();
        colorCountArray = new int[boardParameters.ColorList.Count];
    }

    private void OnHexagonCleared(int i, int j)
    {
        if (!Active || HasBeenJustSpawned) {return;}
        if (i != IndexI || j >= IndexJ) {return;}
        
        DOTween.Kill(myTransform);
        CurrentTargetIndexJ--;
        
        CurrentTargetHeight = BoardCreator.Instance.GetHexagonYPosition(i, CurrentTargetIndexJ);

        myTransform.DOMoveY(CurrentTargetHeight, boardParameters.OldHexagonFallingDuration).OnComplete(() =>
        {
            boardOperator.RemoveHexagonFromBoardHexagonsList(this, IndexI, IndexJ);
            IndexJ = CurrentTargetIndexJ;
            boardOperator.AddHexagonToBoardHexagonsList(this, IndexI, IndexJ);
        });
        

    }

    public void Initialize(float currentTargetHeight)
    {
        Active = true;
        myTransform = transform;
        CurrentTargetHeight = currentTargetHeight;
        CurrentTargetIndexJ = IndexJ;
        SetTouchingHexagonIndexes();
    }

    private void SetTouchingHexagonIndexes()
    {
        neighborHexagonIndexes = new List<int[]>();

        neighborHexagonIndexes.Add(new int[] { IndexI - 1, IndexJ + (IndexI + 1) % 2 }); // On top left
        neighborHexagonIndexes.Add(new int[] { IndexI - 1, IndexJ - 1 + (IndexI + 1) % 2 }); // On bottom left
        neighborHexagonIndexes.Add(new int[] { IndexI, IndexJ - 1 }); // On bottom
        neighborHexagonIndexes.Add(new int[] { IndexI + 1, IndexJ - 1 + (IndexI + 1) % 2 }); // On bottom right
        neighborHexagonIndexes.Add(new int[] { IndexI + 1, IndexJ + (IndexI + 1) % 2 }); // On top right   
        neighborHexagonIndexes.Add(new int[] { IndexI, IndexJ + 1 }); // On top
    }

    public bool CheckHexagonalGroup(bool initialCheck)
    {
        if (!initialCheck) {SetTouchingHexagonIndexes();}
        
        HaveHexagonalGroup = false;
        hexagonalGroup.Clear();
        
        var previousHexagonWasOfMyColor = false;
        
        for (var i = 0; i < neighborHexagonIndexes.Count; i++)
        {
            if (color != boardOperator.GetHexagonColorOnIndex(neighborHexagonIndexes[i]))
            {
                previousHexagonWasOfMyColor = false;
                continue;
            }

            if (previousHexagonWasOfMyColor)
            {
                hexagonalGroup.Add(neighborHexagonIndexes[i]);
                hexagonalGroup.Add(neighborHexagonIndexes[i-1]);
                HaveHexagonalGroup = true;
                continue;
            }
            
            previousHexagonWasOfMyColor = true;
        }

        if (!previousHexagonWasOfMyColor || color != boardOperator.GetHexagonColorOnIndex(neighborHexagonIndexes[0])) { return HaveHexagonalGroup; }
        
        hexagonalGroup.Add(neighborHexagonIndexes[neighborHexagonIndexes.Count - 1]); 
        hexagonalGroup.Add(neighborHexagonIndexes[0]);
        HaveHexagonalGroup = true;

        return HaveHexagonalGroup;
    }

    public bool CheckPotentialHexagonalGroup()
    {
        Array.Clear(colorCountArray, 0, boardParameters.ColorList.Count);
        
        for (var i = 0; i < neighborHexagonIndexes.Count; i++)
        {
            var neighborHexagonColor = boardOperator.GetHexagonColorOnIndex(neighborHexagonIndexes[i]);

            if (neighborHexagonColor == Color.clear) {continue;}
            
            colorCountArray[boardParameters.ColorList.IndexOf(neighborHexagonColor)]++;
        }

        return colorCountArray.Max() >= 3;
    }
    
    protected virtual void OnDisable()
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
