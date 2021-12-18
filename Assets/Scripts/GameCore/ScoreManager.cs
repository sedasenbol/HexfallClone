using Operators;
using SO;
using UI;
using UnityEngine;

namespace GameCore
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField] private ScoreParametersScriptableObject scoreParameters;
    
        private float score;
        private bool active;
        private int previouslySpawnedBombCounter;
    
        public void UpdateScore()
        {
            if (!active) {return;}
        
            score += scoreParameters.ScorePerClearedHexagon;
            UIManager.Instance.UpdateScore(score);
        }

        public bool ShouldSpawnBomb()
        {
            if (score < (previouslySpawnedBombCounter + 1) * scoreParameters.BombHexagonSpawnScore) {return false;}

            previouslySpawnedBombCounter++;
        
            return true;
        }

        private void OnEnable()
        {
            HexagonalGroupFinder.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;
        }

        private void OnAllInitialHexagonalGroupsCleared()
        {
            active = true;
        }
    
 
        private void OnDisable()
        {
            HexagonalGroupFinder.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
        }

        public float Score => score;
    }
}
