using System;
using Operators;
using TMPro;
using UnityEngine;

namespace HexagonalPiece
{
    public class Bomb : Hexagon
    {
        public static event Action OnBombExploded;

        [SerializeField] private TMP_Text counterText;

        private int counter;

        public void SetInitialCounter(int counter)
        {
            this.counter = counter;
            counterText.text = counter.ToString();
        }
    
        protected override void OnEnable()
        {
            base.OnEnable();

            HexagonalGroupFinder.OnPlayerClearedHexagonalGroup += OnPlayerClearedHexagonalGroup;
        }

        private void OnPlayerClearedHexagonalGroup() 
        {
            if (!Active) {return;}
            
            DecreaseCounter();
        }

        private void DecreaseCounter()
        {
            counter--;
            counterText.text = counter.ToString();

            if (counter != 0) {return;}
        
            OnBombExploded?.Invoke();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            HexagonalGroupFinder.OnPlayerClearedHexagonalGroup -= OnPlayerClearedHexagonalGroup;
        }

    
    }
}
