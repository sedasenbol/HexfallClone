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
    [SerializeField] private float hexagonFallingDuration = 2f;
    [SerializeField] private float hexagonFallingAfterSpawnDuration = 2f;

    public float HexagonFallingHeight => hexagonFallingHeight;
    public float HexagonFallingDuration => hexagonFallingDuration;
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
