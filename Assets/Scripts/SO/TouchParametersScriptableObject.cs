using UnityEngine;

namespace SO
{
    [CreateAssetMenu(fileName = "TouchParameters", menuName = "ScriptableObjects/TouchParameters", order = 1)]
    public class TouchParametersScriptableObject : ScriptableObject
    {
        [SerializeField] private float chosenHexagonsScalePercentage = 0.65f;
        [SerializeField] private float dragAngleThreshold;
    
        public float DragAngleThreshold => dragAngleThreshold;
        public float ChosenHexagonsScalePercentage => chosenHexagonsScalePercentage;
    }
}
