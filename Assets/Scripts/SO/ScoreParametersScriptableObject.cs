using UnityEngine;

[CreateAssetMenu(fileName = "ScoreParameters", menuName = "ScriptableObjects/ScoreParameters", order = 1)]
public class ScoreParametersScriptableObject : ScriptableObject
{
    [SerializeField] private float scorePerClearedHexagon = 5f;
    [SerializeField] private float bombHexagonSpawnScore = 1000f;

    public float BombHexagonSpawnScore => bombHexagonSpawnScore;
    public float ScorePerClearedHexagon => scorePerClearedHexagon;
}
