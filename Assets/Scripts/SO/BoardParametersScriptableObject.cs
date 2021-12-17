using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardParameters", menuName = "ScriptableObjects/BoardParameters", order = 1)]
public class BoardParametersScriptableObject : ScriptableObject
{
    
    [SerializeField] private float boardMarginVertical = 3f;
    [SerializeField] private float boardMarginHorizontal = 0.5f;

    [SerializeField] private Transform hexagonTransform;
    [SerializeField] private Transform bombTransform;

    [SerializeField] private int bombSpawnCountPerColor = 5;
    [SerializeField] private int bombCounterMaxValue = 8;
    [SerializeField] private int bombCounterMinValue = 3;
    
    [SerializeField] private int rowCount;
    [SerializeField] private int columnCount;
    [SerializeField] private List<Color> colorList;

    [SerializeField] private float hexagonSpawnHeight = 10f;
    [SerializeField] private float hexagonFallingHeight = -10f;
    [SerializeField] private float oldHexagonFallingDuration = 2f;
    [SerializeField] private float clearedHexagonFallingDuration = 1f;
    [SerializeField] private float hexagonFallingAfterSpawnDuration = 2f;

    [SerializeField] private float initialHexagonGroupCheckDelay = 0.5f;
    [SerializeField] private float hexagonalGroupUnitRotateDuration = 0.5f;

    [SerializeField] private int hexagonalGroupRotateSpinCount = 4;
    
    
    public int HexagonalGroupRotateSpinCount => hexagonalGroupRotateSpinCount;
    public float HexagonalGroupUnitRotateDuration => hexagonalGroupUnitRotateDuration;
    public float InitialHexagonGroupCheckDelay => initialHexagonGroupCheckDelay;
    public float ClearedHexagonFallingDuration => clearedHexagonFallingDuration;
    public float HexagonFallingHeight => hexagonFallingHeight;
    public float OldHexagonFallingDuration => oldHexagonFallingDuration;
    public float HexagonFallingAfterSpawnDuration => hexagonFallingAfterSpawnDuration;
    public float HexagonSpawnHeight => hexagonSpawnHeight;
    public float BoardMarginVertical => boardMarginVertical;
    public float BoardMarginHorizontal => boardMarginHorizontal;
    public int BombSpawnCountPerColor => bombSpawnCountPerColor;
    public int BombCounterMaxValue => bombCounterMaxValue;
    public int BombCounterMinValue => bombCounterMinValue;
    public Transform BombTransform => bombTransform;
    public Transform HexagonTransform => hexagonTransform;
    public int RowCount => rowCount;
    public int ColumnCount => columnCount;
    public List<Color> ColorList => colorList;
}
