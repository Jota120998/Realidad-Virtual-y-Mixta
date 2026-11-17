using System.Collections;
using UnityEngine;
using TMPro;

public class DartManager : MonoBehaviour
{
    [Header("Dianas")]
    [SerializeField] GameObject[] allDarts;
    [SerializeField] float spawnTime;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private TMP_Text dificultyText;
    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private GameObject UiDifficultyPanel;

    [Header("Game Settings")]
    [SerializeField] private int timeLimit = 60;
    [SerializeField] private Color normalTimeColor = Color.white;
    [SerializeField] private Color warningTimeColor = Color.red;
    [SerializeField] private int warningTime = 10;
    
    private int dificulty = 1;
    private int totalScore;
    private float currentTime;
    private bool gameRunning;

    private int bestScore;
    

    void Awake()
    {
        allDarts = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            allDarts[i] = transform.GetChild(i).gameObject;
        }
    }

    void OnEnable()
    {
        Dart.OnDartHit += AddScore;
    }

    void OnDisable()
    {
        Dart.OnDartHit -= AddScore;
    }

    void AddScore(int score, GameObject dart)
    {
        if (!gameRunning) return;

        totalScore += score;
        scoreText.text = "" + totalScore;

        StartCoroutine(ActivateAfterDelay(dart));
    }

    private IEnumerator ActivateAfterDelay(GameObject dart)
    {   
        
        dart.SetActive(false);
        yield return new WaitForSeconds(spawnTime);
        if(gameRunning)
        dart.SetActive(true);
    }

    public void StartGame()
    {   
        Debug.Log("Game Started");
        enableDisableAll(true);
        setDificulty(dificulty);
        totalScore = 0;
        scoreText.text = "0";

        currentTime = timeLimit;
        gameRunning = true;

        UpdateTimeUI();
        StartCoroutine(GameTimer());
    }

    private IEnumerator GameTimer()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimeUI();
            yield return null;
        }

        currentTime = 0;
        UpdateTimeUI();
        EndGame();
    }

    private void UpdateTimeUI()
    {
        int seconds = Mathf.CeilToInt(currentTime);

        int minutes = seconds / 60;
        int remainingSeconds = seconds % 60;

        timeText.text = string.Format("{0:00}:{1:00}", minutes, remainingSeconds);

        if (seconds <= warningTime)
            timeText.color = warningTimeColor;
        else
            timeText.color = normalTimeColor;
    }

    private void EndGame()
    {   
        gameRunning = false;
        UiDifficultyPanel.SetActive(true);
        enableDisableAll(false);
        if(totalScore >= bestScore) 
        {
            bestScore = totalScore;
            bestScoreText.text = "Mejor Puntuación: "+bestScore;
        }
    }

    private void enableDisableAll(bool condition)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            allDarts[i].SetActive(condition);
            
        }
    }

    public void setDificulty(int dif)
    {   
        if(gameRunning) return;
        dificulty = dif;
        for (int i = 0; i < transform.childCount; i++)
        {
            allDarts[i].GetComponent<DartMovement>()?.setDificulty(dif);
        }

        if(dif ==1 ) dificultyText.text = "Dificultad: Fácil";
        if(dif ==2) dificultyText.text = "Dificultad: Media";
        if(dif ==3 ) dificultyText.text = "Dificultad: Dificil";
        
    }
    
}