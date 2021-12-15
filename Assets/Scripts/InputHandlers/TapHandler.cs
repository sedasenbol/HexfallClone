using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class TapHandler : MonoBehaviour
{
    [SerializeField] private HexagonalGroupChooser hexagonalGroupChooser;
    
    private Collider2D[] collider2Ds;
    private int hexagonLayer;
    private bool beforeFirstValidTap = true;

    private void OnEnable()
    {
        collider2Ds = new Collider2D[7];
        hexagonLayer = LayerMask.GetMask("Hexagon");
        
        TouchController.OnPlayerTapEnded += OnPlayerTapEnded;
    }

    private void OnPlayerTapEnded(PointerEventData eventData)
    {
        if (eventData.dragging) {return;}

        if (!beforeFirstValidTap)
        {
            hexagonalGroupChooser.SetAllHexagonScalesToDefault();
        }
        
        var rayOrigin = eventData.pointerCurrentRaycast.worldPosition;
        
        var overlapCircle = Physics2D.OverlapCircleNonAlloc(rayOrigin, BoardCreator.Instance.HexagonXLength / 2f, 
        collider2Ds, hexagonLayer);
        
        if (overlapCircle < 3) {return;}
        
        hexagonalGroupChooser.OnPlayerTapProcessed(collider2Ds, overlapCircle, rayOrigin);

        if (beforeFirstValidTap) { beforeFirstValidTap = false; }
    }



    private void OnDisable()
    {
        collider2Ds = null;

        TouchController.OnPlayerTapEnded -= OnPlayerTapEnded;
    }
}
