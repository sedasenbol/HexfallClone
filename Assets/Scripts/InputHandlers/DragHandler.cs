using Operators;
using SO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace InputHandlers
{
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
        
            if (Mathf.Abs(dragAngle) < touchParameters.DragAngleThreshold)
            {
                previousDragVector = currentDragVector;
                return;
            }
        
            var orientation = dragAngle > 0f ? DragOrientation.Clockwise : DragOrientation.CounterClockwise;
        
            rotatingHexagonalGroupOrderer.OrderAndRotateHexagonalGroup(orientation);
            shouldProcessDrag = false;
        }
    
        private void OnDisable()
        {
            TouchController.OnPlayerDragged -= OnPlayerDragged;
            TouchController.OnPlayerDragBegan -= OnPlayerDragBegan;
            TouchController.OnPlayerDragEnded -= OnPlayerDragEnded;
        }
    }
}