using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static event Action OnPlayButtonClicked;
    public static event Action OnRestartButtonClicked;
    public static event Action OnPauseButtonClicked;
    public static event Action OnResumeButtonClicked;
    public static event Action OnHomePageButtonClicked;

    [SerializeField] private LevelStartParametersScriptableObject levelStartParameters;
    
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject homePageButton;
    [SerializeField] private GameObject startCombiningHexagonsText;

    private YieldInstruction startCombiningHexagonsTextDelay;
    
    private void OnEnable()
    {
        InitialHexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;

        startCombiningHexagonsTextDelay = new WaitForSeconds(levelStartParameters.StartCombiningHexagonsTextDelay);
        
        playButton.SetActive(true);
        restartButton.SetActive(false);
        pauseButton.SetActive(false);
        resumeButton.SetActive(false);
        homePageButton.SetActive(false);
        startCombiningHexagonsText.SetActive(false);
    }

    private void OnAllInitialHexagonalGroupsCleared()
    {
        startCombiningHexagonsText.SetActive(true);

        StartCoroutine(DeactivateStartCombiningHexagonsTextWithDelay());
    }

    private IEnumerator DeactivateStartCombiningHexagonsTextWithDelay()
    {
        yield return startCombiningHexagonsTextDelay;
        
        startCombiningHexagonsText.SetActive(false);
    }

    public void HandlePlayButtonClick()
    {
        playButton.SetActive(false);
        pauseButton.SetActive(true);
        restartButton.SetActive(true);
        startCombiningHexagonsText.SetActive(false);
        
        OnPlayButtonClicked?.Invoke();
    }

    public void HandleRestartButtonClick()
    {
        startCombiningHexagonsText.SetActive(false);
        
        OnRestartButtonClicked?.Invoke();
    }
    
    public void HandlePauseButtonClick()
    {
        pauseButton.SetActive(false);
        restartButton.SetActive(false);
        resumeButton.SetActive(true);
        homePageButton.SetActive(true);
        startCombiningHexagonsText.SetActive(false);
        
        OnPauseButtonClicked?.Invoke();
    }

    public void HandleResumeButtonClick()
    {
        pauseButton.SetActive(true);
        restartButton.SetActive(true);
        resumeButton.SetActive(false);
        homePageButton.SetActive(false);

        OnResumeButtonClicked?.Invoke();
    }

    public void HandleHomePageButtonClick()
    {
        playButton.SetActive(true);
        resumeButton.SetActive(false);
        homePageButton.SetActive(false);
        
        OnHomePageButtonClicked?.Invoke();
    }

    private void OnDisable()
    {
        InitialHexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
    }
}
