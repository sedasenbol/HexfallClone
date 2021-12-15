using System;
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
    [SerializeField] private HexagonalGroupChooser hexagonalGroupChooser;
    [SerializeField] private RotatingHexagonalGroupOrderer rotatingHexagonalGroupOrderer;
    
    private float previousDragAngle;
    private bool shouldProcessDrag;
    
    private void OnEnable()
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
        var dragStartVector = dragStartPosition - hexagonalGroupChooser.ChosenPoint ;
        
        previousDragAngle = Mathf.Atan2(dragStartVector.y, dragStartVector.x) * Mathf.Rad2Deg;
    }

    private void OnPlayerDragged(PointerEventData eventData)
    {
        if (!shouldProcessDrag) {return;}
        
        var currentDragPosition = eventData.pointerCurrentRaycast.worldPosition;
        var currentDragVector = currentDragPosition - hexagonalGroupChooser.ChosenPoint;

        var currentDragAngle = Mathf.Atan2(currentDragVector.y, currentDragVector.x) * Mathf.Rad2Deg;

        if (Mathf.Abs(currentDragAngle - previousDragAngle) > touchParameters.DragAngleThreshold)
        {
            rotatingHexagonalGroupOrderer.OrderAndRotateHexagonalGroup(currentDragAngle < previousDragAngle ? DragOrientation.Clockwise 
                : DragOrientation.CounterClockwise);
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
