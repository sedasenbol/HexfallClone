using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapHandler : MonoBehaviour
{
    [SerializeField] private TouchParametersScriptableObject touchParameters;
  
    private Hexagon[] chosenHexagons;
    private Collider2D[] colliders2D;
    private int hexagonLayer;
    private Vector3 rayOrigin;
    private int[] firstTwoChosenHexagonIndexes;
    private bool beforeFirstValidTap = true;
    private bool hasChosenHexagonGroup;
    private Vector3 chosenPoint;

    private void Awake()
    {
        firstTwoChosenHexagonIndexes = new int[2];
        chosenHexagons = new Hexagon[3];
        colliders2D = new Collider2D[7];
        hexagonLayer = LayerMask.GetMask("Hexagon");
        
        TouchController.OnPlayerTapEnded += OnPlayerTapEnded;
    }

    private void OnPlayerTapEnded(PointerEventData eventData)
    {
        if (eventData.dragging) {return;}

        if (!beforeFirstValidTap)
        {
            SetAllHexagonScalesToDefault();
        }
        
        rayOrigin = eventData.pointerCurrentRaycast.worldPosition;
        
        var overlapCircle = Physics2D.OverlapCircleNonAlloc(rayOrigin, BoardCreator.Instance.HexagonXLength / 2f, 
        colliders2D, hexagonLayer);
        
        if (overlapCircle < 3) {return;}
        
        if (FindTwoChosenHexagons(overlapCircle))
        {
            FindThirdHexagon();
            HandleThreeChosenHexagons();
        }
        
        if (beforeFirstValidTap) { beforeFirstValidTap = false; }
    }


    private void FindThirdHexagon()
    {
        var firstHexagonsNeighbors = new Collider2D[7];
        var secondHexagonNeighbors = new Collider2D[7];
        var contactFilter = new ContactFilter2D().NoFilter();
        
        colliders2D[firstTwoChosenHexagonIndexes[0]].OverlapCollider(contactFilter, firstHexagonsNeighbors);
        colliders2D[firstTwoChosenHexagonIndexes[1]].OverlapCollider(contactFilter, secondHexagonNeighbors);
        
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

    private void SetAllHexagonScalesToDefault()
    {
        for (var i = 0; i < 3; i++)
        {
            chosenHexagons[i].MyTransform.localScale = BoardCreator.Instance.HexagonScale;
        }
    }
    
    private bool FindTwoChosenHexagons(int overlapCircle)
    {
        var hexagons = new Hexagon[overlapCircle];
        var colliderSqrDistances = new float[overlapCircle];
        var rayOrigin2D = new Vector2(rayOrigin.x, rayOrigin.y);
        
        for (var i = 0; i < overlapCircle; i++)
        {
            hexagons[i] = colliders2D[i].transform.GetComponentInParent<Hexagon>();
            colliderSqrDistances[i] = Vector3.SqrMagnitude(colliders2D[i].ClosestPoint(rayOrigin2D) - rayOrigin2D);
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
            
            chosenPoint = chosenHexagons[i].MyTransform.position;
        }

        chosenPoint /= 3f;
    }
    
    private void OnDisable()
    {
        chosenHexagons = null;
        colliders2D = null;

        TouchController.OnPlayerTapEnded -= OnPlayerTapEnded;
    }

    public Hexagon[] ChosenHexagons => chosenHexagons;
    public bool HasChosenHexagonGroup => hasChosenHexagonGroup;
    public Vector3 ChosenPoint => chosenPoint;
}
