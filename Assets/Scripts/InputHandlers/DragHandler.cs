using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum DragOrientation
{
    Clockwise,
    CounterClockwise
}

public class DragHandler : MonoBehaviour
{
    public static event Action<DragOrientation> OnPlayerDragProcessed; 

    [SerializeField] private TouchParametersScriptableObject touchParameters;
    [SerializeField] private TapHandler tapHandler;
    
    private float previousDragAngle;
    private bool shouldProcessDrag;
    
    private void Awake()
    {
        TouchController.OnPlayerDragged += OnPlayerDragged;
        TouchController.OnPlayerDragBegan += OnPlayerDragBegan;
        TouchController.OnPlayerDragEnded += OnPlayerDragEnded;
    }

    private void OnPlayerDragEnded()
    {
        shouldProcessDrag = false;
    }

    private void OnPlayerDragBegan(PointerEventData eventData)
    {
        shouldProcessDrag = true;
        
        var dragStartPosition= eventData.pointerCurrentRaycast.worldPosition;
        var dragStartVector = dragStartPosition - tapHandler.ChosenPoint ;
        
        previousDragAngle = Mathf.Atan2(dragStartVector.y, dragStartVector.x) * Mathf.Rad2Deg;
    }

    private void OnPlayerDragged(PointerEventData eventData)
    {
        if (!shouldProcessDrag) {return;}
        
        var currentDragPosition = eventData.pointerCurrentRaycast.worldPosition;
        var currentDragVector = currentDragPosition - tapHandler.ChosenPoint;

        var currentDragAngle = Mathf.Atan2(currentDragVector.y, currentDragVector.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(currentDragAngle - previousDragAngle) > touchParameters.DragAngleThreshold)
        {
            OnPlayerDragProcessed?.Invoke(currentDragAngle < previousDragAngle ? DragOrientation.Clockwise : DragOrientation.CounterClockwise);
            shouldProcessDrag = false;
            return;
        }

        previousDragAngle = currentDragAngle;
    }
    
    private void OnDisable()
    {
        TouchController.OnPlayerDragged -= OnPlayerDragged;
        TouchController.OnPlayerDragBegan -= OnPlayerDragBegan;
        TouchController.OnPlayerDragEnded -= OnPlayerDragEnded;
    }
}
