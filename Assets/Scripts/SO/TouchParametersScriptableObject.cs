using UnityEngine;

[CreateAssetMenu(fileName = "TouchParameters", menuName = "ScriptableObjects/TouchParameters", order = 1)]
public class TouchParametersScriptableObject : ScriptableObject
{
    [SerializeField] private float tappingRayRadius = 1f;
    [SerializeField] private float tappingRayMaxDistance = 25f;
    [SerializeField] private float chosenHexagonsScalePercentage = 0.65f;
    [SerializeField] private float chosenHexagonsScaleChangeDuration = 0.5f;

    [SerializeReference] private float dragAngleThreshold = 0.5f;


    public float DragAngleThreshold => dragAngleThreshold;
    public float ChosenHexagonsScaleChangeDuration => chosenHexagonsScaleChangeDuration;
    public float TappingRayMaxDistance => tappingRayMaxDistance;
    public float ChosenHexagonsScalePercentage => chosenHexagonsScalePercentage;
    public float TappingRayRadius => tappingRayRadius;
}
