using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexagonalGroupChooser : MonoBehaviour
{
    public static event Action OnAnotherHexagonalGroupChosen;
    
    [SerializeField] private TouchParametersScriptableObject touchParameters;
  
    private Hexagon[] chosenHexagons;
    private int[] firstTwoChosenHexagonIndexes;
    private bool beforeFirstValidTap = true;
    private bool hasChosenHexagonGroup;
    private Vector3 chosenPoint;

    
    private void OnEnable()
    {
        firstTwoChosenHexagonIndexes = new int[2];
        chosenHexagons = new Hexagon[3];
    }

    public void OnPlayerTapProcessed(Collider2D[] collider2Ds, int overlapCircle, Vector3 rayOrigin)
    {
        if (!FindTwoChosenHexagons(collider2Ds, overlapCircle, rayOrigin)) return;
        
        FindThirdHexagon(collider2Ds, rayOrigin);
        HandleThreeChosenHexagons();

        if (!beforeFirstValidTap)
        {
            OnAnotherHexagonalGroupChosen?.Invoke();
            return;
        }
        beforeFirstValidTap = false;
    }

    public void ClearChosenHexagons()
    {
        SetAllHexagonScalesToDefault();
        SetOldHexagonsChosenToFalse();
        
        Array.Clear(chosenHexagons, 0, chosenHexagons.Length);
    }
    
    private void FindThirdHexagon(Collider2D[] collider2Ds, Vector3 rayOrigin)
    {
        var firstHexagonsNeighbors = new Collider2D[7];
        var secondHexagonNeighbors = new Collider2D[7];
        var contactFilter = new ContactFilter2D().NoFilter();
        
        collider2Ds[firstTwoChosenHexagonIndexes[0]].OverlapCollider(contactFilter, firstHexagonsNeighbors);
        collider2Ds[firstTwoChosenHexagonIndexes[1]].OverlapCollider(contactFilter, secondHexagonNeighbors);
        
        var commonNeighbours = firstHexagonsNeighbors.Intersect(secondHexagonNeighbors).ToList();
        
        if (commonNeighbours.Count == 0) { return;}
        if (commonNeighbours[0] == null) { return;}
        if (commonNeighbours.Count == 1) { chosenHexagons[2] = commonNeighbours[0].GetComponentInParent<Hexagon>(); return;}
        if (commonNeighbours[1] == null) { chosenHexagons[2] = commonNeighbours[0].GetComponentInParent<Hexagon>(); return;}

        chosenHexagons[2] = Vector3.SqrMagnitude(commonNeighbours[0].transform.position - rayOrigin) > 
                            Vector3.SqrMagnitude(commonNeighbours[1].transform.position - rayOrigin) 
            ? commonNeighbours[1].GetComponentInParent<Hexagon>()
            : commonNeighbours[0].GetComponentInParent<Hexagon>();

        hasChosenHexagonGroup = true;
    }

    private bool FindTwoChosenHexagons(Collider2D[] collider2Ds, int overlapCircle, Vector3 rayOrigin)
    {
        var hexagons = new Hexagon[overlapCircle];
        var colliderSqrDistances = new float[overlapCircle];
        var rayOrigin2D = new Vector2(rayOrigin.x, rayOrigin.y);
        
        for (var i = 0; i < overlapCircle; i++)
        {
            hexagons[i] = collider2Ds[i].transform.GetComponentInParent<Hexagon>();
            colliderSqrDistances[i] = Vector3.SqrMagnitude(collider2Ds[i].ClosestPoint(rayOrigin2D) - rayOrigin2D);
        }
        
        for (var i = 0; i < 2; i++)
        {
            var minIndex = Array.IndexOf(colliderSqrDistances, colliderSqrDistances.Min());
            firstTwoChosenHexagonIndexes[i] = minIndex;
            colliderSqrDistances[minIndex] = Mathf.Infinity;
        }

        if (!beforeFirstValidTap)
        {
            if (hexagons[firstTwoChosenHexagonIndexes[0]].Chosen || hexagons[firstTwoChosenHexagonIndexes[1]].Chosen)
            {
                SetOldHexagonsChosenToFalse();
                return false;
            }
            SetOldHexagonsChosenToFalse();
        }

        for (var i = 0; i < 2; i++)
        {
            chosenHexagons[i] = hexagons[firstTwoChosenHexagonIndexes[i]];
        }

        return true;
    }

    private void SetOldHexagonsChosenToFalse()
    {
        for (var i = 0; i < 3; i++)
        {
            if (chosenHexagons[i] == null) {continue;}
            
            chosenHexagons[i].Chosen = false;   
        }

        hasChosenHexagonGroup = false;
    }


    private void HandleThreeChosenHexagons()
    {
        chosenPoint = Vector3.zero;
        
        for (var i = 0; i < 3; i++)
        {
            chosenHexagons[i].MyTransform.localScale = BoardCreator.Instance.HexagonScale * touchParameters.ChosenHexagonsScalePercentage;
            chosenHexagons[i].Chosen = true;
            
            chosenPoint += chosenHexagons[i].MyTransform.position;
        }

        chosenPoint /= 3f;
    }
    
    public void SetAllHexagonScalesToDefault()
    {
        for (var i = 0; i < 3; i++)
        {
            if (chosenHexagons[i] == null) {continue;}

            chosenHexagons[i].MyTransform.localScale = BoardCreator.Instance.HexagonScale;
        }
    }
    
    public Hexagon[] ChosenHexagons => chosenHexagons;
    public bool HasChosenHexagonGroup => hasChosenHexagonGroup;
    public Vector3 ChosenPoint => chosenPoint;
        
}
