using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    #region Singleton

    private static BoardManager instance;
    public static BoardManager Instance => instance;

    public static event Action<int, int> OnHexagonCleared;

    private int[] newHexagonArrayPerColumn;
    
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); return; } 
        
        instance = this;
    }

    #endregion

    [SerializeField] private BoardParametersScriptableObject boardParameters;

    private List<Hexagon>[] boardHexagons => BoardCreator.Instance.BoardHexagons;

    private List<int[]> hexagonIndexesToSetInactive;

    public void CheckInitialHexagons()
    {
        hexagonIndexesToSetInactive = new List<int[]>();
        
        for (var i = 0; i < boardHexagons.Length; i++)
        {
            for (var j = 0; j < boardHexagons[i].Count; j++)
            {
                boardHexagons[i][j].Initialize();
                if (!boardHexagons[i][j].CheckHexagonalGroup()) {continue;}
                
                hexagonIndexesToSetInactive.AddRange(boardHexagons[i][j].HexagonalGroup);
                hexagonIndexesToSetInactive.Add(new []{i,j});
                boardHexagons[i][j].haveHexagonalGroup = false;
            }
        }

        ClearHexagonalGroups();
    }

    private void ClearHexagonalGroups()
    {
        foreach (var indexes in hexagonIndexesToSetInactive)
        {
            var currentHexagon = boardHexagons[indexes[0]][indexes[1]];
            
            if (!currentHexagon.Active) {continue;}
            currentHexagon.Active = false;
            
            currentHexagon.MyTransform.DOMoveY(boardParameters.HexagonFallingHeight, boardParameters.HexagonFallingDuration).OnComplete(() =>
                {
                    HexagonPooler.Instance.AddItemBackToThePool(currentHexagon.gameObject, currentHexagon.color);
                    OnHexagonCleared?.Invoke(indexes[0], indexes[1]);
                    AddNewHexagon(indexes[0], boardParameters.RowCount - 1);
                });
        }
    }

    public void AddHexagonToBoardHexagonsList(Hexagon hexagon, int i, int j)
    {
        boardHexagons[i][j] = hexagon;
    }
    
    private void AddNewHexagon(int i, int j)
    {
        var randomColor = boardParameters.ColorList[Random.Range(0, boardParameters.ColorList.Count)];
        var finalPosition = BoardCreator.Instance.GetHexagonPosition(i, j);

        var spawnPosition = new Vector3()
        {
            x = finalPosition.x,
            y = boardParameters.HexagonSpawnHeight,
            z = 0f
        };

        var hexagonTransform = HexagonPooler.Instance.SpawnFromPool(randomColor, spawnPosition);
        hexagonTransform.DOMoveY(finalPosition.y, boardParameters.HexagonFallingAfterSpawnDuration);
        var hexagon = hexagonTransform.GetComponent<Hexagon>();
        hexagon.Initialize();
        hexagon.CurrentTargetIndexJ = j;
        hexagon.CurrentTargetHeight = finalPosition.y;
        hexagon.color = randomColor;
        hexagon.IndexI = i;
        hexagon.IndexJ = j;
    }

    public Color GetHexagonColorOnIndex(int[] indexes)
    {
        if (indexes[0] < 0 || indexes[0] > boardParameters.ColumnCount - 1 || indexes[1] < 0 ||
            indexes[1] > boardParameters.RowCount - 1) { return Color.clear; }

        return boardHexagons[indexes[0]][indexes[1]].color;
    }
}
