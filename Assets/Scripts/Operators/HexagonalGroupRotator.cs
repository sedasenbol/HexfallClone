using System;
using System.Linq;
using UnityEngine;

public class HexagonalGroupRotator : MonoBehaviour
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;

    [SerializeField] private HexagonChooser hexagonChooser;
    [SerializeField] private BoardOperator boardOperator;
    
    private void Awake()
    {
        DragHandler.OnPlayerDragProcessed += OnPlayerDragProcessed;
    }

    private void OnPlayerDragProcessed(DragOrientation orientation)
    {
        if (!hexagonChooser.HasChosenHexagonGroup) {return;}
        
        OrderChosenHexagons(orientation, out var hexagonTransforms, out var orderedIndexes);

        boardOperator.RotateAndCheckHexagonalGroupMultipleTimes(hexagonChooser.ChosenHexagons, hexagonTransforms, orderedIndexes);
    }

    private void OrderChosenHexagons(DragOrientation orientation, out Transform[] hexagonTransforms, out int[] orderedIndexes)
    {
        var currentHexagonAngles = new float[3];
        hexagonTransforms = new Transform[3];
        orderedIndexes = new int[3];

        for (var i = 0; i < 3; i++)
        {
            hexagonTransforms[i] = hexagonChooser.ChosenHexagons[i].MyTransform;

            currentHexagonAngles[i] = Vector3.SignedAngle(hexagonChooser.ChosenPoint, hexagonTransforms[i].position, Vector3.forward);
        }

        for (var i = 0; i < 3; i++)
        {
            var currentMinIndex = Array.IndexOf(currentHexagonAngles,
                orientation == DragOrientation.Clockwise ? currentHexagonAngles.Max() : currentHexagonAngles.Min());
            orderedIndexes[i] = currentMinIndex;

            currentHexagonAngles[currentMinIndex] = orientation == DragOrientation.Clockwise ? -Mathf.Infinity : Mathf.Infinity;
        }
    }

    
    
    private void OnDisable()
    {
        DragHandler.OnPlayerDragProcessed -= OnPlayerDragProcessed;
    }
}
