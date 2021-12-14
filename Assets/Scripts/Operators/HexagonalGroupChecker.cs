using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class HexagonalGroupChecker : MonoBehaviour
{
    public static event Action<int, int> OnHexagonCleared;
    public static event Action OnAllInitialHexagonalGroupsCleared;
    
    #region Singleton

    private static HexagonalGroupChecker instance;
    public static HexagonalGroupChecker Instance => instance;
   
    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); return; } 
        
        instance = this;
    }

    #endregion

    [SerializeField] private HexagonChooser hexagonChooser;
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private BoardOperator boardOperator;
    
    private YieldInstruction waitForNewHexagonsFall;
    private List<Hexagon>[] boardHexagons => BoardCreator.Instance.BoardHexagons;
    private List<int[]> hexagonIndexesToSetInactive;
    private List<Hexagon> recentlySpawnedHexagons;

    private void OnEnable()
    {
        waitForNewHexagonsFall = new WaitForSeconds(boardParameters.ClearedHexagonFallingDuration + boardParameters
        .HexagonFallingAfterSpawnDuration + boardParameters.HexagonGroupCheckDelay);
        
        hexagonIndexesToSetInactive = new List<int[]>();
        recentlySpawnedHexagons = new List<Hexagon>();
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
                CheckHexagonalGroupAroundIndex(i, j, true);
            }
        }

        RemoveDuplicatesFromHexagonIndexesToSetInactiveList();

        while (ClearHexagonalGroups(true)) { StartCoroutine(CheckInitialHexagonsWithDelay()); return;}

        OnAllInitialHexagonalGroupsCleared?.Invoke();
    }

    private void RemoveDuplicatesFromHexagonIndexesToSetInactiveList()
    {
        hexagonIndexesToSetInactive = hexagonIndexesToSetInactive.Distinct().ToList();
    }

    public bool CheckHexagonalGroupAfterRotate(Hexagon[] rotatingHexagons)
    {
        SetOldHexagonsHasBeenJustSpawnedToFalse();

        hexagonIndexesToSetInactive.Clear();

        for (var i = 0; i < 3; i++)
        {
            CheckHexagonalGroupAroundIndex(rotatingHexagons[i].IndexI, rotatingHexagons[i].IndexJ, false);
        }

        RemoveDuplicatesFromHexagonIndexesToSetInactiveList();
        
        return ClearHexagonalGroups(false);
    }

    private void SetOldHexagonsHasBeenJustSpawnedToFalse()
    {
        for (var i = 0; i < recentlySpawnedHexagons.Count; i++)
        {
            recentlySpawnedHexagons[i].HasBeenJustSpawned = false;
        }
    }

    private void CheckHexagonalGroupAroundIndex(int i, int j, bool initialCheck)
    {
        if (!boardHexagons[i][j].CheckHexagonalGroup(initialCheck)) { return; }

        hexagonIndexesToSetInactive.AddRange(boardHexagons[i][j].HexagonalGroup);
        hexagonIndexesToSetInactive.Add(new[] { i, j });
        boardHexagons[i][j].HaveHexagonalGroup = false;
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

    private bool ClearHexagonalGroups(bool initialHexagonCheck)
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

            StartCoroutine(RemoveHexagon(currentHexagon, indexes));
        }

        recentlySpawnedHexagons.Clear();
        AddNewHexagonsToClearedColumns(clearedHexagonsInColumns);

        if (cleared && !initialHexagonCheck) { hexagonChooser.ClearChosenHexagons(); }
        
        return cleared;
    }

    private IEnumerator RemoveHexagon(Hexagon currentHexagon, int[] indexes)
    {
        

        currentHexagon.MyTransform.DOMoveY(boardParameters.HexagonFallingHeight, boardParameters.ClearedHexagonFallingDuration)
            .OnComplete(() =>
            {
                HexagonPooler.Instance.AddItemBackToThePool(currentHexagon.gameObject, currentHexagon.color);
                OnHexagonCleared?.Invoke(indexes[0], indexes[1]);
                boardOperator.RemoveHexagonFromBoardHexagonsList(currentHexagon, indexes[0], indexes[1]);
            });
        
        yield return null;

    }
    
    private void AddNewHexagonsToClearedColumns(List<int> clearedHexagonsInColumns)
    {
        for (var i = 0; i < boardParameters.ColumnCount; i++)
        {
            for (var j = 0; j < clearedHexagonsInColumns[i]; j++)
            {
                recentlySpawnedHexagons.Add(AddNewHexagon(i,boardParameters.RowCount - j - 1));
            }
        }
    }

    private IEnumerator CheckInitialHexagonsWithDelay()
    {
        yield return waitForNewHexagonsFall;

        CheckInitialHexagons();
    }

    private Hexagon AddNewHexagon(int i, int j)
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
        hexagon.IndexI = i;
        hexagon.IndexJ = j;
        hexagon.Initialize();
        hexagonTransform.DOMoveY(finalPosition.y, boardParameters.HexagonFallingAfterSpawnDuration);
        hexagon.CurrentTargetHeight = finalPosition.y;

        hexagon.color = randomColor;
        boardOperator.AddHexagonToBoardHexagonsList(hexagon, i, j);

        return hexagon;
    }


}
