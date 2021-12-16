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
    [SerializeField] private TouchParametersScriptableObject touchParameters;
    [SerializeField] private HexagonalGroupChooser hexagonalGroupChooser;
    [SerializeField] private RotatingHexagonalGroupOrderer rotatingHexagonalGroupOrderer;

    private Vector3 previousDragVector;
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
        previousDragVector = dragStartPosition - hexagonalGroupChooser.ChosenPoint ;
    }

    private void OnPlayerDragged(PointerEventData eventData)
    {
        if (!shouldProcessDrag) {return;}
        
        var currentDragPosition = eventData.pointerCurrentRaycast.worldPosition;
        var currentDragVector = currentDragPosition - hexagonalGroupChooser.ChosenPoint;
        
        var dragAngle = Vector3.SignedAngle(currentDragVector, previousDragVector, Vector3.forward);
        
        if (Mathf.Abs(dragAngle) > touchParameters.DragAngleThreshold)
        {
            rotatingHexagonalGroupOrderer.OrderAndRotateHexagonalGroup(dragAngle > 0f ? DragOrientation.Clockwise 
                : DragOrientation.CounterClockwise);
            shouldProcessDrag = false;
            return;
        }

        previousDragVector = currentDragVector;
    }
    
    private void OnDisable()
    {
        TouchController.OnPlayerDragged -= OnPlayerDragged;
        TouchController.OnPlayerDragBegan -= OnPlayerDragBegan;
        TouchController.OnPlayerDragEnded -= OnPlayerDragEnded;
    }
}
