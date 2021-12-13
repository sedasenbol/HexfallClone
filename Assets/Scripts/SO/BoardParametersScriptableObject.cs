using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardParameters", menuName = "ScriptableObjects/BoardParameters", order = 1)]
public class BoardParametersScriptableObject : ScriptableObject
{
    
    [SerializeField] private float boardMarginVertical = 3f;
    [SerializeField] private float boardMarginHorizontal = 0.5f;

    [SerializeField] private Transform hexagonTransform;
    [SerializeField] private int poolSizePerColor = 100;
    
    [SerializeField] private int rowCount;
    [SerializeField] private int columnCount;
    [SerializeField] private List<Color> colorList;

    [SerializeField] private float hexagonSpawnHeight = 10f;
    [SerializeField] private float hexagonFallingHeight = -10f;
    [SerializeField] private float oldHexagonFallingDuration = 2f;
    [SerializeField] private float clearedHexagonUnitFallingDuration = 1f;
    [SerializeField] private float hexagonFallingAfterSpawnDuration = 2f;

    [SerializeField] private float hexagonGroupCheckDelay = 0.5f;
    [SerializeField] private float hexagonalGroupRotateDuration = 0.5f;

    public float HexagonalGroupRotateDuration => hexagonalGroupRotateDuration;
    public float HexagonGroupCheckDelay => hexagonGroupCheckDelay;
    public float ClearedHexagonUnitFallingDuration => clearedHexagonUnitFallingDuration;
    public float HexagonFallingHeight => hexagonFallingHeight;
    public float OldHexagonFallingDuration => oldHexagonFallingDuration;
    public float HexagonFallingAfterSpawnDuration => hexagonFallingAfterSpawnDuration;
    public float HexagonSpawnHeight => hexagonSpawnHeight;
    public float BoardMarginVertical => boardMarginVertical;
    public float BoardMarginHorizontal => boardMarginHorizontal;
    public int PoolSizePerColor => poolSizePerColor;
    public Transform HexagonTransform => hexagonTransform;
    public int RowCount => rowCount;
    public int ColumnCount => columnCount;
    public List<Color> ColorList => colorList;
}
