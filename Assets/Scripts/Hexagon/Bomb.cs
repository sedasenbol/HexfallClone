using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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

        HexagonalGroupChecker.OnPlayerClearedHexagonalGroup += OnPlayerClearedHexagonalGroup;
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

        HexagonalGroupChecker.OnPlayerClearedHexagonalGroup -= OnPlayerClearedHexagonalGroup;
    }

    
}
