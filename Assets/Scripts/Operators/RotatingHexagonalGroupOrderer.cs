using System;
using System.Linq;
using InputHandlers;
using UnityEngine;

namespace Operators
{
    public class RotatingHexagonalGroupOrderer : MonoBehaviour
    {
        [SerializeField] private HexagonalGroupChooser hexagonalGroupChooser;
        [SerializeField] private HexagonalGroupRotator hexagonalGroupRotator;
    
        public void OrderAndRotateHexagonalGroup(DragOrientation orientation)
        {
            if (!hexagonalGroupChooser.HasChosenHexagonGroup) {return;}
        
            if (!OrderChosenHexagons(orientation, out var hexagonTransforms, out var orderedIndexes)) {return;}

            hexagonalGroupRotator.RotateAndCheckHexagonalGroupMultipleTimes(hexagonalGroupChooser.ChosenHexagons, hexagonTransforms, orderedIndexes);
        }

        private bool OrderChosenHexagons(DragOrientation orientation, out Transform[] hexagonTransforms, out int[] orderedIndexes)
        {
            var currentHexagonAngles = new float[3];
            hexagonTransforms = new Transform[3];
            orderedIndexes = new int[3];

            for (var i = 0; i < 3; i++)
            {
                hexagonTransforms[i] = hexagonalGroupChooser.ChosenHexagons[i].MyTransform;

                if (hexagonTransforms[i] == null) { return false;}

                currentHexagonAngles[i] = Vector3.SignedAngle(hexagonTransforms[i].position - hexagonalGroupChooser.ChosenPoint, 
                    Vector3.right, Vector3.forward);
            }

            for (var i = 0; i < 3; i++)
            {
                var currentIndex = Array.IndexOf(currentHexagonAngles,
                    orientation == DragOrientation.Clockwise ? currentHexagonAngles.Min() : currentHexagonAngles.Max());
                orderedIndexes[i] = currentIndex;

                currentHexagonAngles[currentIndex] = orientation == DragOrientation.Clockwise ? Mathf.Infinity : -Mathf.Infinity;
            }

            return true;
        }
    }
}
