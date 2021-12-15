using System;
using System.Linq;
using UnityEngine;

public class RotatingHexagonalGroupOrderer : MonoBehaviour
{
    [SerializeField] private HexagonalGroupChooser hexagonalGroupChooser;
    [SerializeField] private HexagonalGroupRotator hexagonalGroupRotator;

    private bool isFirstRotatingHexagonalGroup = true;
    
    public void OrderAndRotateHexagonalGroup(DragOrientation orientation)
    {
        if (!hexagonalGroupChooser.HasChosenHexagonGroup) {return;}
        
        OrderChosenHexagons(orientation, out var hexagonTransforms, out var orderedIndexes);

        hexagonalGroupRotator.StopPreviouslyRotatingHexagonalGroup();
        hexagonalGroupRotator.RotateAndCheckHexagonalGroupMultipleTimes(hexagonalGroupChooser.ChosenHexagons, hexagonTransforms, orderedIndexes);
    }

    private void OrderChosenHexagons(DragOrientation orientation, out Transform[] hexagonTransforms, out int[] orderedIndexes)
    {
        var currentHexagonAngles = new float[3];
        hexagonTransforms = new Transform[3];
        orderedIndexes = new int[3];

        for (var i = 0; i < 3; i++)
        {
            hexagonTransforms[i] = hexagonalGroupChooser.ChosenHexagons[i].MyTransform;

            currentHexagonAngles[i] = Vector3.SignedAngle(hexagonalGroupChooser.ChosenPoint, hexagonTransforms[i].position, Vector3.forward);
        }

        for (var i = 0; i < 3; i++)
        {
            var currentMinIndex = Array.IndexOf(currentHexagonAngles,
                orientation == DragOrientation.Clockwise ? currentHexagonAngles.Max() : currentHexagonAngles.Min());
            orderedIndexes[i] = currentMinIndex;

            currentHexagonAngles[currentMinIndex] = orientation == DragOrientation.Clockwise ? -Mathf.Infinity : Mathf.Infinity;
        }
    }
}
