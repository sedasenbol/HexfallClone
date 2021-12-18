using System.Collections.Generic;
using HexagonalPiece;
using SO;
using UnityEngine;

namespace Operators
{
    public class HexagonPooler : Singleton<HexagonPooler>
    {
        [SerializeField] private BoardParametersScriptableObject boardParameters;
        [SerializeField] private Transform hexagonalPieceContainerTransform;

        private Dictionary<Color, Queue<GameObject>> hexagonPoolDict;
        private Dictionary<Color, Queue<GameObject>> bombPoolDict;

        private int poolSizePerHexagonColor;
    
        public void PrepareHexagonPools()
        {
            SetPoolSizePerHexagonColor();
            InitializeHexagonPoolDict();
            InitializeBombPoolDict();
        }

        public Transform SpawnFromPool(Color color, Vector3 position, bool shouldSpawnBomb)
        {
            if (hexagonPoolDict[color].Count == 0) { Debug.LogError($"Pool size of {color} item is insufficient."); return null; }

            var objectSpawned = shouldSpawnBomb ? bombPoolDict[color].Dequeue() : hexagonPoolDict[color].Dequeue();
        
            objectSpawned.SetActive(true);
        
            var objectSpawnedTransform = objectSpawned.transform;
            objectSpawnedTransform.position = position;
        
            if (shouldSpawnBomb)
            {
                objectSpawned.GetComponent<Bomb>().SetInitialCounter(Random.Range(boardParameters.BombCounterMinValue,
                    boardParameters.BombCounterMaxValue + 1));
            }
        
            return objectSpawnedTransform;
        }

        public void AddItemBackToThePool(GameObject hexagonalPieceGameObject, Color color)
        {
            hexagonalPieceGameObject.SetActive(false);
            hexagonPoolDict[color].Enqueue(hexagonalPieceGameObject);
        }

        private void SetPoolSizePerHexagonColor()
        {
            poolSizePerHexagonColor = boardParameters.RowCount * boardParameters.ColumnCount + 1;
        }

        private void InitializeHexagonPoolDict()
        {
            hexagonPoolDict = new Dictionary<Color, Queue<GameObject>>();
        
            InitializeHexagonalPiecePoolDict(hexagonPoolDict, true);
        }

        private void InitializeBombPoolDict()
        {
            bombPoolDict = new Dictionary<Color, Queue<GameObject>>();
        
            InitializeHexagonalPiecePoolDict(bombPoolDict, false);
        }
    
        private void InitializeHexagonalPiecePoolDict(Dictionary<Color, Queue<GameObject>> hexagonalPiecePoolDict, bool isHexagon)
        {
            for (var i = 0; i < boardParameters.ColorList.Count; i++)
            {
                var poolSize = isHexagon ? poolSizePerHexagonColor : boardParameters.BombSpawnCountPerColor;
            
                var newHexagonalPiecePool = new Queue<GameObject>();

                InitializeHexagonPool(poolSize, boardParameters.ColorList[i], newHexagonalPiecePool, isHexagon);

                hexagonalPiecePoolDict[boardParameters.ColorList[i]] = newHexagonalPiecePool;
            }
        }

        private void InitializeHexagonPool(int poolSize, Color color, Queue<GameObject> newHexagonPool, bool isHexagon)
        {
            for (var j = 0; j < poolSize; j++)
            {
                var hexagonalPieceTransform = isHexagon ? boardParameters.HexagonTransform : boardParameters.BombTransform;
            
                var obj = Instantiate(hexagonalPieceTransform, hexagonalPieceContainerTransform).gameObject;
            
                obj.SetActive(false);
                obj.GetComponentInChildren<SpriteRenderer>().color = color;
                obj.GetComponent<Hexagon>().MyColor = color;
        
                newHexagonPool.Enqueue(obj);
            }
        }
    }
}
