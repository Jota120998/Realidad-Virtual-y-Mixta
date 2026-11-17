using UnityEngine;
using TMPro;

using System.Collections.Generic;
using System.Collections;

public class DartManagerAR : MonoBehaviour
{
    [Header("Dianas")]
    [SerializeField] float spawnTime; // Tiempo entre cada aparición de diana
    [SerializeField] private GameObject dartPrefabs; // Prefabs de las dianas

    private List<GameObject> activeDarts = new List<GameObject>();
    public int maxDarts = 10;
    private int actualDarts = 0;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private TMP_Text bestScoreText;
    [SerializeField] private GameObject UIPanelStart;

    [Header("Game Settings")]
    [SerializeField] private int timeLimit = 60;
    [SerializeField] private Color normalTimeColor = Color.white;
    [SerializeField] private Color warningTimeColor = Color.red;
    [SerializeField] private int warningTime = 10;

    private int totalScore;
    private float currentTime = 0f;
    private bool gameRunning;

    private MeshFilter meshFilter;
    private List<Vector3> validPoints = new List<Vector3>();
    private List<Vector3> validNormals = new List<Vector3>();
    private int bestScore = 0;

        void Awake()
    {
        Debug.Log("DartManagerAR Awake");
    }

     void OnEnable()
    {   
        ARMeshAnalyzer.OnMeshProcessed += SetPointsAndNormals; // Suscribirse al evento de ARMeshAnalyzer
        Dart.OnDartHit += AddScore;
    }

    void OnDisable()
    {   
        ARMeshAnalyzer.OnMeshProcessed -= SetPointsAndNormals; // Suscribirse al evento de ARMeshAnalyzer
        Dart.OnDartHit -= AddScore;
    }

    void AddScore(int score, GameObject dart)
    {
        if (!gameRunning) return;
        actualDarts--;
        totalScore += score;
        scoreText.text = "" + totalScore;
    }
    
    public void StartGame()
    {
        totalScore = 0;
        actualDarts = 0;
        currentTime = timeLimit;
        gameRunning = true;
        scoreText.text = "0";
        UIPanelStart.SetActive(false);
        StartCoroutine(GameTimer());
        Debug.Log("Game Started");
    }

    //Bucle del juego
    private IEnumerator GameTimer()
    {
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            UpdateTimeUI();
            Debug.Log("spawn time: " + spawnTime + " current time: " + currentTime  + " actual darts: " + actualDarts);
            SpawnDart();
            yield return null; // Esperar hasta el siguiente frame
            
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
        foreach (GameObject dart in activeDarts)
        {
            Destroy(dart);
        }
        activeDarts.Clear();
        UIPanelStart.SetActive(true);
        
        
        if(totalScore >= bestScore) 
        {
            bestScore = totalScore;
            bestScoreText.text = "Mejor Puntuación: "+bestScore;
        }
    }


    private void SetPointsAndNormals(List<Vector3> points,List<Vector3> normals )
    {
        validPoints = points;
        validNormals = normals;
    }

    public void SpawnDart()
    {
        
        for (int i = actualDarts; i < maxDarts; i++)
        {   
        actualDarts++;
        // Elegir un punto aleatorio de la lista de puntos válidos
        Vector3 spawnPoint = validPoints[UnityEngine.Random.Range(0, validPoints.Count)];

        // Instanciar la diana en el punto seleccionado
        GameObject dart = Instantiate(dartPrefabs, spawnPoint, Quaternion.LookRotation(validNormals[validPoints.IndexOf(spawnPoint)]));
        activeDarts.Add(dart);
    }
    }
}
