using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Orientation
{
    Horizontal,
    Vertical
}

public class BoardCreator : MonoBehaviour
{
    #region Singleton

    private static BoardCreator instance;
    public static BoardCreator Instance => instance;


    private void Awake()
    {
        if (instance != null && instance != this) { Destroy(this.gameObject); return; } 
        
        instance = this;
    }

    #endregion
    
    [SerializeField] private BoardParametersScriptableObject boardParameters;

    private Camera mainCam;
    private Vector3 bottomLeftScreenWorldPos;
    
    private float hexagonXLength;
    private float hexagonSideLength;
    private float hexagonYLength;
    private float limitXHexagonScaleMultiplier;
    private float limitYHexagonScaleMultiplier;
    private Orientation limitOrientation;
    private Vector3 hexagonScale;
    private float heightDifferenceBetweenHexagons;

    private List<Hexagon>[] boardHexagons;

    private void OnEnable()
    {
        mainCam = Camera.main;
    }

    public void CreateBoard()
    {
        GetHexagonLengths();
        SetLimitingHexagonScaleMultipliers();
        SetHexagonScaleValue();
        
        CreateInitialHexagons();
        
        SetHeightDifferenceBetweenHexagons();
    }

    private void GetHexagonLengths()
    {
        var hexagonRendererBounds = boardParameters.HexagonTransform.GetComponentInChildren<Renderer>().bounds;

        hexagonXLength = hexagonRendererBounds.size.x;
        hexagonYLength = hexagonRendererBounds.size.y;
        hexagonSideLength = hexagonXLength / (1 + Mathf.Sqrt(2));
    }
    
    private void SetLimitingHexagonScaleMultipliers()
    {
        bottomLeftScreenWorldPos = mainCam.ViewportToWorldPoint(Vector3.zero);
        var topRightScreenWorldPos = mainCam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        
        var horizontalScreenLength = topRightScreenWorldPos.x - bottomLeftScreenWorldPos.x - 2 * boardParameters.BoardMarginHorizontal;
        var verticalScreenLength = topRightScreenWorldPos.y - bottomLeftScreenWorldPos.y - 2 * boardParameters.BoardMarginVertical;

        limitXHexagonScaleMultiplier = horizontalScreenLength / ((boardParameters.ColumnCount + 2) / 2 * hexagonXLength +
                                                                 (boardParameters.ColumnCount + 1) / 2 * hexagonSideLength);
        limitYHexagonScaleMultiplier = verticalScreenLength / ((boardParameters.RowCount + 0.5f) * hexagonYLength);
    
        limitOrientation = limitXHexagonScaleMultiplier < limitYHexagonScaleMultiplier
            ? Orientation.Horizontal
            : Orientation.Vertical;
    }
    
    private void SetHexagonScaleValue()
    {
        var currentScale = boardParameters.HexagonTransform.localScale;
        
        hexagonScale = limitOrientation == Orientation.Horizontal
            ? currentScale * limitXHexagonScaleMultiplier / hexagonXLength
            : currentScale * limitYHexagonScaleMultiplier / hexagonYLength;

        hexagonXLength *= hexagonScale.x / currentScale.x;
        hexagonYLength *= hexagonScale.y / currentScale.y;

        hexagonSideLength = hexagonXLength / (1 + Mathf.Sqrt(2));
    }
    
    private void CreateInitialHexagons()
    {
        boardHexagons = new List<Hexagon>[boardParameters.ColumnCount];
        
        for (var i = 0; i < boardParameters.ColumnCount; i++)
        {
            boardHexagons[i] = new List<Hexagon>();
            
            for (var j = 0; j < boardParameters.RowCount; j++)
            {
                var hexagonTransform = HexagonPooler.Instance.SpawnFromPool(GetRandomColor(), GetHexagonPosition(i, j));
                hexagonTransform.localScale = hexagonScale;
                
                var hexagon = hexagonTransform.GetComponent<Hexagon>();

                hexagon.IndexI = i;
                hexagon.IndexJ = j;
                
                boardHexagons[i].Add(hexagon);
            }
        }
    }

    private Color GetRandomColor()
    {
        return boardParameters.ColorList[Random.Range(0, boardParameters.ColorList.Count)];
    }
    
    public Vector3 GetHexagonPosition(int i, int j)
    {
        return new Vector3()
        {
            x = GetHexagonXPosition(i),
            y = GetHexagonYPosition(i, j),
            z = 0f
        };
    }

    private float GetHexagonYPosition(int i, int j)
    {
        return bottomLeftScreenWorldPos.y + boardParameters.BoardMarginVertical / 2f +
               ((i + 1) % 2 * 0.5f + j + 0.5f) * hexagonYLength;
    }

    private void SetHeightDifferenceBetweenHexagons()
    {
        // Same for every column and row
        heightDifferenceBetweenHexagons = GetHexagonYPosition(0, 1) - GetHexagonYPosition(0, 0);
    }
    
    private float GetHexagonXPosition(int i)
    {
        return bottomLeftScreenWorldPos.x + boardParameters.BoardMarginHorizontal / 2f + (i + 1) / 2f * hexagonXLength +
            (i / 2f) * hexagonSideLength;
    }

    private void OnDisable()
    {
        mainCam = null;
        boardHexagons = null;
    }

    public float HeightDifferenceBetweenHexagons => heightDifferenceBetweenHexagons;
    public List<Hexagon>[] BoardHexagons => boardHexagons;
}
