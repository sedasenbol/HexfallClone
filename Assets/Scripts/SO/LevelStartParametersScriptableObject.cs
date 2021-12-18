using UnityEngine;

[CreateAssetMenu(fileName = "LevelStartParameters", menuName = "ScriptableObjects/LevelStartParameters", order = 1)]
public class LevelStartParametersScriptableObject : ScriptableObject
{
    [SerializeField] private int tweenCapacity = 100;
    [SerializeField] private int sequencesCapacity = 5;

    [SerializeField] private float startCombiningHexagonsTextDelay = 1.5f;

    public float StartCombiningHexagonsTextDelay => startCombiningHexagonsTextDelay;
    public int TweenCapacity => tweenCapacity;
    public int SequencesCapacity => sequencesCapacity;

}
