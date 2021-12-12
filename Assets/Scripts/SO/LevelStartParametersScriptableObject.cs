using UnityEngine;

[CreateAssetMenu(fileName = "LevelStartParameters", menuName = "ScriptableObjects/LevelStartParameters", order = 1)]
public class LevelStartParametersScriptableObject : ScriptableObject
{
    [SerializeField] private int tweenersCapacity = 100;
    [SerializeField] private int sequencesCapacity = 0;

    [SerializeField] private float initialHexagonCheckDelay = 0.5f;
    [SerializeField] private float startCombiningHexagonsTextDelay = 1.5f;

    public float StartCombiningHexagonsTextDelay => startCombiningHexagonsTextDelay;
    public float InitialHexagonCheckDelay => initialHexagonCheckDelay;
    public int TweenersCapacity => tweenersCapacity;
    public int SequencesCapacity => sequencesCapacity;

}
