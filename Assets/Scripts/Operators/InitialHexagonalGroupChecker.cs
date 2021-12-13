using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class InitialHexagonalGroupChecker : MonoBehaviour
{
    public static event Action<int, int> OnHexagonCleared;
    public static event Action OnAllInitialHexagonalGroupsCleared;
    
    #region Singleton

    private static InitialHexagonalGroupChecker instance;
    public static InitialHexagonalGroupChecker Instance => instance;

    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); return; } 
        
        instance = this;
    }

    #endregion

    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private BoardOperator boardOperator;
    
    private YieldInstruction waitForNewHexagonsFall;
    private List<Hexagon>[] boardHexagons => BoardCreator.Instance.BoardHexagons;
    
    private List<int[]> hexagonIndexesToSetInactive;


    private void OnEnable()
    {
        waitForNewHexagonsFall = new WaitForSeconds(boardParameters.ClearedHexagonUnitFallingDuration + boardParameters
        .HexagonFallingAfterSpawnDuration + boardParameters.HexagonGroupCheckDelay);
        
        hexagonIndexesToSetInactive = new List<int[]>();
    }

    public void CheckInitialHexagons()
    {
        hexagonIndexesToSetInactive.Clear();
        
        for (var i = 0; i < boardHexagons.Length; i++)
        {
            for (var j = 0; j < boardHexagons[i].Count; j++)
            {
                boardHexagons[i][j].Initialize();
                boardHexagons[i][j].HasBeenJustSpawned = false;
                if (!boardHexagons[i][j].CheckHexagonalGroup()) {continue;}
                
                hexagonIndexesToSetInactive.AddRange(boardHexagons[i][j].HexagonalGroup);
                hexagonIndexesToSetInactive.Add(new []{i,j});
                boardHexagons[i][j].HaveHexagonalGroup = false;
            }
        }
        
        OrderHexagonIndexesToSetInactive();
        ClearHexagonalGroups();
    }

    private void OrderHexagonIndexesToSetInactive()
    {
        var hexagonIndexesToSetInactiveCopy = new List<int[]>(hexagonIndexesToSetInactive);
        
        hexagonIndexesToSetInactive.Clear();

        for (var i = 0; i < boardParameters.ColumnCount; i++)
        {
            for (var j = 0; j < boardParameters.RowCount; j++)
            {
                var currentIndex = new[] { i, j };

                if (!hexagonIndexesToSetInactiveCopy.Any(index => index.SequenceEqual(currentIndex))) {continue;}

                hexagonIndexesToSetInactiveCopy.RemoveAll(index => index.SequenceEqual(currentIndex));
                hexagonIndexesToSetInactive.Add(currentIndex);
            }
        }
    }

    private void ClearHexagonalGroups()
    {
        var clearedHexagonsInColumns = Enumerable.Repeat(0, boardParameters.ColumnCount).ToList();

        var cleared = false;

        for (var i = 0; i < hexagonIndexesToSetInactive.Count; i++)
        {
            var indexes = hexagonIndexesToSetInactive[i];

            var currentHexagon = boardHexagons[indexes[0]][indexes[1]];
            
            if (!currentHexagon.Active) {continue;}

            cleared = true;
            clearedHexagonsInColumns[indexes[0]]++;
            currentHexagon.Active = false;
            
            currentHexagon.MyTransform.DOMoveY(boardParameters.HexagonFallingHeight, boardParameters.ClearedHexagonUnitFallingDuration)
                .OnComplete(() =>
                {
                    HexagonPooler.Instance.AddItemBackToThePool(currentHexagon.gameObject, currentHexagon.color);
                    OnHexagonCleared?.Invoke(indexes[0], indexes[1]);
                    boardOperator.RemoveHexagonFromBoardHexagonsList(currentHexagon, indexes[0], indexes[1]);
                });
        }

        AddHexagonsToClearedColumns(clearedHexagonsInColumns);
        
        if (cleared) { StartCoroutine(CheckInitialHexagonsWithDelay()); return;}
        
        OnAllInitialHexagonalGroupsCleared?.Invoke();
    }

    private void AddHexagonsToClearedColumns(List<int> clearedHexagonsInColumns)
    {
        for (var i = 0; i < boardParameters.ColumnCount; i++)
        {
            for (var j = 0; j < clearedHexagonsInColumns[i]; j++)
            {
                AddNewHexagon(i,boardParameters.RowCount - j - 1);
            }
        }
    }

    private IEnumerator CheckInitialHexagonsWithDelay()
    {
        yield return waitForNewHexagonsFall;

        CheckInitialHexagons();
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
        hexagonTransform.localScale = BoardCreator.Instance.HexagonScale;
        var hexagon = hexagonTransform.GetComponent<Hexagon>();
        hexagon.HasBeenJustSpawned = true;
        hexagon.Initialize();
        hexagonTransform.DOMoveY(finalPosition.y, boardParameters.HexagonFallingAfterSpawnDuration);
        hexagon.CurrentTargetHeight = finalPosition.y;
        hexagon.IndexI = i;
        hexagon.IndexJ = j;
        hexagon.color = randomColor;
        boardOperator.AddHexagonToBoardHexagonsList(hexagon, i, j);
    }


}
