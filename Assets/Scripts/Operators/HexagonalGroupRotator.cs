using System.Collections;
using DG.Tweening;
using UnityEngine;

public class HexagonalGroupRotator : MonoBehaviour
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private HexagonalGroupChecker hexagonalGroupChecker;
    [SerializeField] private BoardOperator boardOperator;

    private int hexagonalGroupRotateCounter;
    private YieldInstruction waitForPreviousRotateFinish;
    private bool hasRotatingHexagonGroup;

    private void Awake()
    {
        waitForPreviousRotateFinish = new WaitForSeconds(boardParameters.HexagonalGroupUnitRotateDuration);
    }

    public void RotateAndCheckHexagonalGroupMultipleTimes(Hexagon[] chosenHexagons, Transform[] hexagonTransforms, int[] 
    orderedIndexes)
    {
        hexagonalGroupRotateCounter = 0;
        hasRotatingHexagonGroup = true;

        RotateAndCheckHexagonalGroup(chosenHexagons, hexagonTransforms, orderedIndexes);
    }

    private void RotateAndCheckHexagonalGroup(Hexagon[] chosenHexagons, Transform[] hexagonTransforms, int[] orderedIndexes)
    {
        if (hexagonalGroupRotateCounter == boardParameters.HexagonalGroupRotateSpinCount * 3)
        {
            hasRotatingHexagonGroup = false;
            return;
        }
        
        hexagonalGroupRotateCounter++;
        
        RotateHexagonalGroup(chosenHexagons, hexagonTransforms, orderedIndexes);

        StartCoroutine(CheckHexagonalGroupAfterRotateWithDelay(chosenHexagons, hexagonTransforms, orderedIndexes));
    }

    private IEnumerator CheckHexagonalGroupAfterRotateWithDelay(Hexagon[] chosenHexagons, Transform[] hexagonTransforms, int[] 
    orderedIndexes)
    {
        yield return waitForPreviousRotateFinish;

        if (hexagonalGroupChecker.CheckHexagonalGroupAfterRotate(chosenHexagons))
        {
            hasRotatingHexagonGroup = false;
            yield break;
        }

        RotateAndCheckHexagonalGroup(chosenHexagons, hexagonTransforms, orderedIndexes);
    }

    private void RotateHexagonalGroup(Hexagon[] chosenHexagons, Transform[] hexagonTransforms, int[] orderedIndexes)
    {
        var firstHexagonPosition = hexagonTransforms[orderedIndexes[0]].position;
        var firsHexagonIndexes = new int[] { chosenHexagons[orderedIndexes[0]].IndexI, chosenHexagons[orderedIndexes[0]].IndexJ };

        for (var i = 0; i < 2; i++)
        {
            var currentHexagon = chosenHexagons[orderedIndexes[i]];
            var nextHexagon = chosenHexagons[orderedIndexes[i + 1]];

            var currentHexagonTransform = hexagonTransforms[orderedIndexes[i]];
            var nextHexagonTransform = hexagonTransforms[orderedIndexes[i + 1]];

            currentHexagonTransform.DOMove(nextHexagonTransform.position, boardParameters.HexagonalGroupUnitRotateDuration);
            
            currentHexagon.IndexI = nextHexagon.IndexI;
            currentHexagon.IndexJ = nextHexagon.IndexJ;
            currentHexagon.CurrentTargetIndexJ = nextHexagon.IndexJ;
            
            boardOperator.AddHexagonToBoardHexagonsList(currentHexagon, nextHexagon.IndexI,nextHexagon.IndexJ);
        }

        hexagonTransforms[orderedIndexes[2]].DOMove(firstHexagonPosition, boardParameters.HexagonalGroupUnitRotateDuration);
        
        chosenHexagons[orderedIndexes[2]].IndexI = firsHexagonIndexes[0];
        chosenHexagons[orderedIndexes[2]].IndexJ = firsHexagonIndexes[1];
        chosenHexagons[orderedIndexes[2]].CurrentTargetIndexJ = firsHexagonIndexes[1];
        
        boardOperator.AddHexagonToBoardHexagonsList(chosenHexagons[orderedIndexes[2]], firsHexagonIndexes[0],firsHexagonIndexes[1]);
    }

    public bool HasRotatingHexagonGroup => hasRotatingHexagonGroup;

}