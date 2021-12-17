using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotentialValidMoveChecker : MonoBehaviour
{
    public static event Action OnNoPotentialMoveLeft; 

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
        
        OnNoPotentialMoveLeft?.Invoke();
    }
}
