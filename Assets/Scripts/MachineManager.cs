using UnityEngine;
using TMPro;
using System;

public class MachineManager : MonoBehaviour
{
    public static MachineManager Instance;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("Temporizador")]
    public float totalTime = 60f;

    [Header("Topo Manager")]
    public TopoManager topoManager;

    private int score = 0;
    private float timeRemaining;
    private bool timerRunning = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if(scoreText == null)
            Debug.LogError("ScoreText no asignado en MachineManager");

        if(timerText == null)
            Debug.LogError("TimerText no asignado en MachineManager");

        if(topoManager == null)
            Debug.LogError("TopoManager no asignado en MachineManager");
    }

    private void OnEnable()
    {
        Topo.OnTopoHit += AddScore;
    }

    private void OnDisable()
    {
        Topo.OnTopoHit -= AddScore;
    }

    private void Update()
    {
        if (!timerRunning) return;

        timeRemaining -= Time.deltaTime;
        if(timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            StopGame();
        }

        UpdateTimerUI();
    }

    // --- Score ---
    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if(scoreText != null)
            scoreText.text = score.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    // --- Timer ---
    private void UpdateTimerUI()
    {
        if(timerText != null)
            timerText.text = Mathf.Ceil(timeRemaining).ToString();
    }

    private void ResetTimer()
    {
        timeRemaining = totalTime;
        UpdateTimerUI();
    }

    // --- Control de partida ---
    public void StartGame()
    {
        ResetScore();
        ResetTimer();
        timerRunning = true;

        if(topoManager != null)
            topoManager.StartSpawning();
    }

    public void StopGame()
    {
        timerRunning = false;

        if(topoManager != null)
            topoManager.StopSpawning();
    }
}