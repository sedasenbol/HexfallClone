using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class BoardOperator : MonoBehaviour
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private HexagonalGroupChecker hexagonalGroupChecker;

    private int hexagonalGroupRotateCounter;
    private YieldInstruction waitForPreviousRotateFinish;

    private void Awake()
    {
        waitForPreviousRotateFinish = new WaitForSeconds(boardParameters.HexagonalGroupUnitRotateDuration + 0.1f);
    }

    private List<Hexagon>[] boardHexagons => BoardCreator.Instance.BoardHexagons;

    public void AddHexagonToBoardHexagonsList(Hexagon hexagon, int i, int j)
    {
        boardHexagons[i][j] = hexagon;
    }

    public void RemoveHexagonFromBoardHexagonsList(Hexagon hexagon, int i, int j)
    {
        if (boardHexagons[i][j] != hexagon) {return;}
        
        boardHexagons[i][j] = null;
    }
    
    public Color GetHexagonColorOnIndex(int[] indexes)
    {
        if (indexes[0] < 0 || indexes[0] > boardParameters.ColumnCount - 1 || 
            indexes[1] < 0 || indexes[1] > boardParameters.RowCount - 1) 
        { return Color.clear; }
        
        return boardHexagons[indexes[0]][indexes[1]].color; 
    }

    public void RotateAndCheckHexagonalGroupMultipleTimes(Hexagon[] chosenHexagons, Transform[] hexagonTransforms, int[] 
    orderedIndexes)
    {
        hexagonalGroupRotateCounter = 0;

        RotateAndCheckHexagonalGroup(chosenHexagons, hexagonTransforms, orderedIndexes);
    }

    private void RotateAndCheckHexagonalGroup(Hexagon[] chosenHexagons, Transform[] hexagonTransforms, int[] orderedIndexes)
    {
        if (hexagonalGroupRotateCounter == boardParameters.HexagonalGroupRotateSpinCount * 3) {return;}
        
        hexagonalGroupRotateCounter++;
        
        SwitchIndexesAndPositionsInHexagonalGroup(chosenHexagons, hexagonTransforms, orderedIndexes);

        StartCoroutine(CheckHexagonalGroupAfterRotateWithDelay(chosenHexagons, hexagonTransforms, orderedIndexes));
    }

    private IEnumerator CheckHexagonalGroupAfterRotateWithDelay(Hexagon[] chosenHexagons, Transform[] hexagonTransforms, int[] 
    orderedIndexes)
    {
        yield return waitForPreviousRotateFinish;

        if (hexagonalGroupChecker.CheckHexagonalGroupAfterRotate(chosenHexagons)) { yield break; }

        RotateAndCheckHexagonalGroup(chosenHexagons, hexagonTransforms, orderedIndexes);
    }

    private void SwitchIndexesAndPositionsInHexagonalGroup(Hexagon[] chosenHexagons, Transform[] hexagonTransforms,
        int[] orderedIndexes)
    {
        var firstHexagonPosition = hexagonTransforms[orderedIndexes[0]].position;
        var firsHexagonIndexes = new int[] { chosenHexagons[orderedIndexes[0]].IndexI, chosenHexagons[orderedIndexes[0]].IndexJ };

        for (var i = 0; i < 2; i++)
        {
            var currentHexagon = chosenHexagons[orderedIndexes[i]];
            var nextHexagon = chosenHexagons[orderedIndexes[i + 1]];

            var currentHexagonTransform = hexagonTransforms[orderedIndexes[i]];
            var nextHexagonTransform = hexagonTransforms[orderedIndexes[i + 1]];

            /*currentHexagonTransform.DOMove(nextHexagonTransform.position, boardParameters.HexagonalGroupUnitRotateDuration)
                .OnComplete(() =>
                {
            
                    currentHexagon.IndexI = nextHexagon.IndexI;
                    currentHexagon.IndexJ = nextHexagon.IndexJ;
                
                    boardHexagons[nextHexagon.IndexI][nextHexagon.IndexJ] = currentHexagon;
                });
            */
            currentHexagon.IndexI = nextHexagon.IndexI;
            currentHexagon.IndexJ = nextHexagon.IndexJ;
                
            boardHexagons[nextHexagon.IndexI][nextHexagon.IndexJ] = currentHexagon;
        }

        hexagonTransforms[orderedIndexes[2]].DOMove(firstHexagonPosition, boardParameters.HexagonalGroupUnitRotateDuration)
            .OnComplete(() =>
            {
                chosenHexagons[orderedIndexes[2]].IndexI = firsHexagonIndexes[0];
                chosenHexagons[orderedIndexes[2]].IndexJ = firsHexagonIndexes[1];
                
                boardHexagons[firsHexagonIndexes[0]][firsHexagonIndexes[1]] = chosenHexagons[orderedIndexes[2]];
            });
    }
}