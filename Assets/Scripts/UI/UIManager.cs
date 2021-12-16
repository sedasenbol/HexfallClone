using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    public static event Action OnPlayButtonClicked;
    public static event Action OnRestartButtonClicked;
    public static event Action OnPauseButtonClicked;
    public static event Action OnResumeButtonClicked;
    public static event Action OnHomePageButtonClicked;

    [SerializeField] private LevelStartParametersScriptableObject levelStartParameters;

    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject resumeButton;
    [SerializeField] private GameObject homePageButton;
    [SerializeField] private GameObject gameOverRestartButton;
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private GameObject startCombiningHexagonsText;

    private YieldInstruction startCombiningHexagonsTextDelay;

    public void UpdateScore(float score)
    {
        scoreText.text = score.ToString("F0");
    }
    
    private void OnEnable()
    {
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared += OnAllInitialHexagonalGroupsCleared;

        startCombiningHexagonsTextDelay = new WaitForSeconds(levelStartParameters.StartCombiningHexagonsTextDelay);
        
        scoreText.gameObject.SetActive(false);
        playButton.SetActive(true);
        restartButton.SetActive(false);
        pauseButton.SetActive(false);
        resumeButton.SetActive(false);
        homePageButton.SetActive(false);
        startCombiningHexagonsText.SetActive(false);
    }

    private void OnAllInitialHexagonalGroupsCleared()
    {
        scoreText.gameObject.SetActive(true);
        scoreText.text = "0";

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
        scoreText.gameObject.SetActive(false);
        startCombiningHexagonsText.SetActive(false);
        gameOverRestartButton.SetActive(false);
        gameOverScoreText.gameObject.SetActive(false);
        homePageButton.SetActive(false);
        
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
        scoreText.gameObject.SetActive(false);
        playButton.SetActive(true);
        resumeButton.SetActive(false);
        restartButton.SetActive(false);
        homePageButton.SetActive(false);
        gameOverRestartButton.SetActive(false);
        gameOverScoreText.gameObject.SetActive(false);
        
        OnHomePageButtonClicked?.Invoke();
    }

    private void OnDisable()
    {
        HexagonalGroupChecker.OnAllInitialHexagonalGroupsCleared -= OnAllInitialHexagonalGroupsCleared;
    }

    public void LoadGameOverScreen(float score)
    {
        pauseButton.SetActive(false);
        resumeButton.SetActive(false);
        restartButton.SetActive(false);
        homePageButton.SetActive(true);
        scoreText.gameObject.SetActive(false);
        gameOverRestartButton.SetActive(true);
        gameOverScoreText.gameObject.SetActive(true);

        gameOverScoreText.text = $"Your score is: {score}";
    }
}
