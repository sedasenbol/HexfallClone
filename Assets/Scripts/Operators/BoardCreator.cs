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

public class BoardCreator : Singleton<BoardCreator>
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;

    private Camera mainCam;
    private Vector3 bottomLeftScreenWorldPos;
    private Vector3 topRightScreenWorldPos;
    
    private float hexagonXLength;
    private float hexagonYLength;
    private float limitXHexagonScaleMultiplier;
    private float limitYHexagonScaleMultiplier;
    private Vector3 hexagonScale;
    private Vector3 boardOffset;
    private float heightDifferenceBetweenHexagons;
    
    private void OnEnable()
    {
        mainCam = Camera.main;
    }

    public void CreateBoard()
    {
        GetHexagonLengths();
        SetLimitingHexagonScale();
        SetBoardOffset();
        
        CreateInitialHexagons();
        
        SetHeightDifferenceBetweenHexagons();
    }

    private void GetHexagonLengths()
    {
        var hexagonRendererBounds = boardParameters.HexagonTransform.GetComponentInChildren<Renderer>().bounds;

        hexagonXLength = hexagonRendererBounds.size.x;
        hexagonYLength = hexagonRendererBounds.size.y;
    }
    
    private void SetLimitingHexagonScale()
    {
        bottomLeftScreenWorldPos = mainCam.ViewportToWorldPoint(Vector3.zero);
        topRightScreenWorldPos = mainCam.ViewportToWorldPoint(new Vector3(1f, 1f, 0f));
        
        var horizontalScreenLength = topRightScreenWorldPos.x - bottomLeftScreenWorldPos.x - boardParameters.BoardMarginHorizontal;
        var verticalScreenLength = topRightScreenWorldPos.y - bottomLeftScreenWorldPos.y - boardParameters.BoardMarginVertical;

        limitXHexagonScaleMultiplier = horizontalScreenLength / GetCurrentGridXLength();
        limitYHexagonScaleMultiplier = verticalScreenLength / GetCurrentGridYLength();
        
        AdjustHexagonLengths();
        SetBoardOffset();
    }

    private float GetCurrentGridYLength()
    {
        return (GetHexagonYPosition(0,boardParameters.RowCount - 1) - GetHexagonYPosition(0,0) + 1.5f * hexagonYLength);
    }

    private float GetCurrentGridXLength()
    {
        return (GetHexagonXPosition(boardParameters.ColumnCount - 1) - GetHexagonXPosition(0) + hexagonXLength);
    }

    private void AdjustHexagonLengths()
    {
        var currentScale = boardParameters.HexagonTransform.localScale;
        var limitMultiplier = limitXHexagonScaleMultiplier < limitYHexagonScaleMultiplier
            ? limitXHexagonScaleMultiplier
            : limitYHexagonScaleMultiplier;
        
        hexagonScale = limitMultiplier * currentScale;

        hexagonXLength *= limitMultiplier;
        hexagonYLength *= limitMultiplier;
    }

    private void SetBoardOffset()
    {
        var gridLength = new Vector3(GetCurrentGridXLength(), GetCurrentGridYLength(), 0f);
        var marginOffset = new Vector3(boardParameters.BoardMarginHorizontal, boardParameters.BoardMarginVertical, 0f);

        boardOffset = topRightScreenWorldPos - bottomLeftScreenWorldPos - gridLength - marginOffset;
    }
    
    private void CreateInitialHexagons()
    {
        BoardHexagons = new List<Hexagon>[boardParameters.ColumnCount];
        
        for (var i = 0; i < boardParameters.ColumnCount; i++)
        {
            BoardHexagons[i] = new List<Hexagon>();
            
            for (var j = 0; j < boardParameters.RowCount; j++)
            {
                var hexagonTransform = HexagonPooler.Instance.SpawnFromPool(GetRandomColor(), GetHexagonPosition(i, j), false);
                hexagonTransform.localScale = hexagonScale;
                
                var hexagon = hexagonTransform.GetComponent<Hexagon>();

                hexagon.IndexI = i;
                hexagon.IndexJ = j;
                
                BoardHexagons[i].Add(hexagon);
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

    public float GetHexagonYPosition(int i, int j)
    {
        return boardOffset.y / 2f + bottomLeftScreenWorldPos.y + boardParameters.BoardMarginVertical / 2f +
               ((1 + (i+1) % 2) * 0.5f + j) * hexagonYLength;
    }

    private void SetHeightDifferenceBetweenHexagons()
    {
        // Same for every column and row
        heightDifferenceBetweenHexagons = GetHexagonYPosition(0, 1) - GetHexagonYPosition(0, 0);
    }
    
    private float GetHexagonXPosition(int i)
    {
        return boardOffset.x / 2f + bottomLeftScreenWorldPos.x + boardParameters.BoardMarginHorizontal / 2f + hexagonXLength / 
        2f + i * 0.75f * hexagonXLength;
    }

    private void OnDisable()
    {
        mainCam = null;
        BoardHexagons = null;
    }

    public float HexagonXLength => hexagonXLength;
    public Vector3 HexagonScale => hexagonScale;
    public float HeightDifferenceBetweenHexagons => heightDifferenceBetweenHexagons;
    public List<Hexagon>[] BoardHexagons { get; set; }
}
