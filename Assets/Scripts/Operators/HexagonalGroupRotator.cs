using UnityEngine;

public class HexagonalGroupRotator : MonoBehaviour
{
    [SerializeField] private TapHandler tapHandler;
    [SerializeField] private BoardOperator boardOperator;
    
    
    private void Awake()
    {
        DragHandler.OnPlayerDragProcessed += OnPlayerDragProcessed;
    }

    private void OnPlayerDragProcessed(DragOrientation orientation)
    {
        if (!tapHandler.HasChosenHexagonGroup) {return;}
        
        boardOperator.RotateHexagonalGroup(tapHandler.ChosenHexagons, tapHandler.ChosenPoint, orientation);
    }

    private void OnDisable()
    {
        DragHandler.OnPlayerDragProcessed -= OnPlayerDragProcessed;
    }
}
