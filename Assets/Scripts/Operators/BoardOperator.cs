using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BoardOperator : MonoBehaviour
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;

    private readonly Vector3 clockwiseRotationVector = new Vector3(0f, 0f, -120f);
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
        Debug.Log(boardParameters == null);

        if (indexes[0] < 0 || indexes[0] > boardParameters.ColumnCount - 1 || 
            indexes[1] < 0 || indexes[1] > boardParameters.RowCount - 1) 
        { return Color.clear; }
        
        return boardHexagons[indexes[0]][indexes[1]].color; 
    }

    public void RotateHexagonalGroup(Hexagon[] tapHandlerChosenHexagons, Vector3 tapHandlerChosenPoint, DragOrientation orientation)
    {
        var hexagonalGroupRotationVector = orientation == DragOrientation.Clockwise ? clockwiseRotationVector : -clockwiseRotationVector;

        for (var i = 0; i < tapHandlerChosenHexagons.Length; i++)
        {
          //  tapHandlerChosenHexagons[i].MyTransform.parent = currentHexagonalGroupTransform;
        }

        //currentHexagonalGroupTransform.DORotate(hexagonalGroupRotationVector, boardParam.HexagonalGroupRotateDuration);
    }
}