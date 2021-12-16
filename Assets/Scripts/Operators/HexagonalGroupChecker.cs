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
    public static event Action OnPlayerClearedHexagonalGroup;
    public static event Action OnAllInitialHexagonalGroupsCleared;

    [SerializeField] private HexagonalGroupChooser hexagonalGroupChooser;
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private BoardOperator boardOperator;
    [SerializeField] private ScoreManager scoreManager;
    
    private YieldInstruction waitForNewHexagonsFall;
    private List<Hexagon>[] boardHexagons => BoardCreator.Instance.BoardHexagons;
    private List<int[]> hexagonIndexesToSetInactive;
    private List<Hexagon> recentlySpawnedHexagons;

    private void OnEnable()
    {
        waitForNewHexagonsFall = new WaitForSeconds(boardParameters.ClearedHexagonFallingDuration + boardParameters
        .HexagonFallingAfterSpawnDuration + boardParameters.InitialHexagonGroupCheckDelay);
        
        hexagonIndexesToSetInactive = new List<int[]>();
        recentlySpawnedHexagons = new List<Hexagon>();
    }

    public void CheckInitialHexagonGroups()
    {
        if (CheckHexagonalGroupsInAllBoard(true)) return;

        OnAllInitialHexagonalGroupsCleared?.Invoke();
    }

    private bool CheckHexagonalGroupsInAllBoard(bool initialCheck)
    {
        hexagonIndexesToSetInactive.Clear();

        for (var i = 0; i < boardHexagons.Length; i++)
        {
            for (var j = 0; j < boardHexagons[i].Count; j++)
            {
                boardHexagons[i][j].Initialize(BoardCreator.Instance.GetHexagonYPosition(i, j));
                boardHexagons[i][j].HasBeenJustSpawned = false;
                CheckHexagonalGroupAroundIndex(i, j, true);
            }
        }

        RemoveDuplicatesFromHexagonIndexesToSetInactiveList();

        while (ClearHexagonalGroups(true))
        {
            StartCoroutine(initialCheck ? CheckInitialHexagonGroupsWithDelay() : CheckHexagonalGroupsInAllBoardWithDelay());
            return true;
        }

        return false;
    }

    private void RemoveDuplicatesFromHexagonIndexesToSetInactiveList()
    {
        hexagonIndexesToSetInactive = hexagonIndexesToSetInactive.Distinct().ToList();
    }

    public bool CheckHexagonalGroupAfterRotate(Hexagon[] rotatingHexagons)
    {
        SetOldHexagonsHasBeenJustSpawnedToFalse();

        hexagonIndexesToSetInactive.Clear();

        var hexagonsChecked = new List<Hexagon>();
        hexagonsChecked.AddRange(rotatingHexagons);
        
        for (var i = 0; i < 3; i++)
        {
            CheckHexagonalGroupAroundIndex(rotatingHexagons[i].IndexI, rotatingHexagons[i].IndexJ, false);
        }

        for (var i = 0; i < hexagonIndexesToSetInactive.Count; i++)
        {
            var hexagonToSetInactive = boardHexagons[hexagonIndexesToSetInactive[i][0]][hexagonIndexesToSetInactive[i][1]];
            
            if (hexagonsChecked.Contains(hexagonToSetInactive)) {break;}
            
            CheckHexagonalGroupAroundIndex(hexagonIndexesToSetInactive[i][0], hexagonIndexesToSetInactive[i][1], false);
        }
        
        RemoveDuplicatesFromHexagonIndexesToSetInactiveList();

        if (ClearHexagonalGroups(false))
        {
            OnPlayerClearedHexagonalGroup?.Invoke();
            StartCoroutine(CheckHexagonalGroupsInAllBoardWithDelay());
            return true;
        }

        return false;
    }

    private IEnumerator CheckHexagonalGroupsInAllBoardWithDelay()
    {
        yield return waitForNewHexagonsFall;

        CheckHexagonalGroupsInAllBoard(false);
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

        if (!cleared) return false;
        
        recentlySpawnedHexagons.Clear();
        AddNewHexagonsToClearedColumns(clearedHexagonsInColumns);
        if (!initialHexagonCheck) { hexagonalGroupChooser.ClearChosenHexagons(); }

        return true;
    }

    private IEnumerator RemoveHexagon(Hexagon currentHexagon, int[] indexes)
    {
        yield return null;

        currentHexagon.MyTransform.DOMoveY(boardParameters.HexagonFallingHeight, boardParameters.ClearedHexagonFallingDuration);
        
        scoreManager.UpdateScore();
        HexagonPooler.Instance.AddItemBackToThePool(currentHexagon.gameObject, currentHexagon.color);
        boardOperator.RemoveHexagonFromBoardHexagonsList(currentHexagon, indexes[0], indexes[1]);
        OnHexagonCleared?.Invoke(indexes[0], indexes[1]);
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

    private IEnumerator CheckInitialHexagonGroupsWithDelay()
    {
        yield return waitForNewHexagonsFall;

        CheckInitialHexagonGroups();
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

        var hexagonTransform = HexagonPooler.Instance.SpawnFromPool(randomColor, spawnPosition, scoreManager.ShouldSpawnBomb());
        hexagonTransform.localScale = BoardCreator.Instance.HexagonScale;
        var hexagon = hexagonTransform.GetComponent<Hexagon>();
        hexagon.HasBeenJustSpawned = true;
        hexagon.IndexI = i;
        hexagon.IndexJ = j;
        hexagon.Initialize(finalPosition.y);
        hexagonTransform.DOMoveY(finalPosition.y, boardParameters.HexagonFallingAfterSpawnDuration);
        hexagon.color = randomColor;
        boardOperator.AddHexagonToBoardHexagonsList(hexagon, i, j);

        return hexagon;
    }


}
