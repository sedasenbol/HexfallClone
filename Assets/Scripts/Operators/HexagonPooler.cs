using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class HexagonPooler : Singleton<HexagonPooler>
{
    [SerializeField] private BoardParametersScriptableObject boardParameters;
    [SerializeField] private Transform hexagonContainerTransform;

    private Dictionary<Color, Queue<GameObject>> hexagonPoolDict;

    public void PrepareHexagonPools()
    {
        InitializeHexagonPoolDict();
    }
    
    public Transform SpawnFromPool(Color color, Vector3 position)
    {
        if (hexagonPoolDict[color].Count == 0) { Debug.LogError($"Pool size of {color} item is insufficient."); return null; }
        
        var objectSpawned = hexagonPoolDict[color].Dequeue();
        objectSpawned.SetActive(true);
        
        var objectSpawnedTransform = objectSpawned.transform;
        objectSpawnedTransform.position = position;

        return objectSpawnedTransform;
    }

    public void AddItemBackToThePool(GameObject itemGameObject, Color color)
    {
        itemGameObject.SetActive(false);
        hexagonPoolDict[color].Enqueue(itemGameObject);
    }
    
    private void OnEnable()
    {
        GameManager.OnGameSceneLoaded += OnGameSceneLoaded;
    }

    private void OnGameSceneLoaded()
    {
        InitializeHexagonPoolDict();
    }

    private void InitializeHexagonPoolDict()
    {
        hexagonPoolDict = new Dictionary<Color, Queue<GameObject>>();
        
        var poolSize = boardParameters.PoolSizePerColor;
        
        for (var i = 0; i < boardParameters.ColorList.Count; i++)
        {
            var newHexagonPool = new Queue<GameObject>(poolSize);

            InitializeHexagonPool(poolSize, i, newHexagonPool);

            hexagonPoolDict[boardParameters.ColorList[i]] = newHexagonPool;
        }
    }

    private void InitializeHexagonPool(int poolSize, int i, Queue<GameObject> newHexagonPool)
    {
        for (var j = 0; j < poolSize; j++)
        {
            var obj = Instantiate(boardParameters.HexagonTransform, hexagonContainerTransform).gameObject;
            obj.SetActive(false);
            obj.GetComponentInChildren<SpriteRenderer>().color = boardParameters.ColorList[i];
            obj.GetComponent<Hexagon>().color = boardParameters.ColorList[i];
            
            newHexagonPool.Enqueue(obj);
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameSceneLoaded -= OnGameSceneLoaded;

        hexagonPoolDict = null;
    }
}
