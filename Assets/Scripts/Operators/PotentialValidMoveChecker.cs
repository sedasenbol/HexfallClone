using System;
using System.Collections.Generic;
using HexagonalPiece;
using UnityEngine;

namespace Operators
{
    public class PotentialValidMoveChecker : MonoBehaviour
    {
        public static event Action OnNoPotentialValidMoveLeft; 

        private List<Hexagon>[] boardHexagons => BoardCreator.Instance.BoardHexagons;
    
        public void CheckIfThereIsAnyPotentialValidMoveLeft()
        {
            for (var i = 0; i < boardHexagons.Length; i++)
            {
                for (var j = 0; j < boardHexagons[i].Count; j++)
                {
                    if (boardHexagons[i][j].CheckPotentialHexagonalGroup()) {return;}
                }
            }
        
            OnNoPotentialValidMoveLeft?.Invoke();
        }
    }
}
