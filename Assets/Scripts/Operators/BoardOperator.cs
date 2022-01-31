using System.Collections.Generic;
using HexagonalPiece;
using SO;
using UnityEngine;

namespace Operators
{
    public class BoardOperator : MonoBehaviour
    {
        [SerializeField] private BoardParametersScriptableObject boardParameters;
    
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

            return boardHexagons[indexes[0]][indexes[1]] == null ? Color.clear : boardHexagons[indexes[0]][indexes[1]].MyColor;
        }
    }
}