using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(MeshFilter))]
public class ARMeshAnalyzer : MonoBehaviour
{
    [Header("AR Mesh Manager")]
    public ARMeshManager arMeshManager;
    
    [Header("Configuración")]
    [Tooltip("Tiempo antes de congelar la malla")]
    public float freezeDelay = 3f;

    [Header("Configuración de paredes")]

    [Tooltip("Valor mínimo de la normal en Y para considerar que es pared")] //si y = 0 es suelo, si y = 1 es techo, si y es cercano a 0 es pared
    [Range(-1f, 1f)]
    public float normalThreshold = 0.5f;

    [Tooltip("Separación mínima entre puntos válidos")]
    public float minDistanceBetweenPoints = 1f;

    public float minGlobalHeight = 0.5f; 

    [Header("Debug")]
    public bool debugDrawPoints = true;

    
    public static event Action<List<Vector3>, List<Vector3>> OnMeshProcessed;


    //Variables privadas

    private static MeshFilter meshFilter;
    private bool meshFrozen = false;
    private float timer = 0f;

    private static List<Vector3> validPoints = new List<Vector3>();
    private static List<Vector3> validnormals = new List<Vector3>();

    void Awake()
    {   
        arMeshManager = FindFirstObjectByType<ARMeshManager>();
        meshFilter = GetComponent<MeshFilter>();
    }

    void LateUpdate()
    {   
        if (!meshFrozen && meshFilter.sharedMesh != null) // Solo procesar si hay una malla válida y no está "congelada"
        ProcessMeshUpdate();
        
        if (debugDrawPoints && validPoints.Count > 0 && meshFrozen)
        {
            OnDebugDraw();
        }
    }

    void ProcessMeshUpdate()
    {
       
        timer += Time.deltaTime;

        // Esperar unos segundos para dejar que AR escanee
        if (timer >= freezeDelay)
        {
            arMeshManager.enabled = false; // Detener actualizaciones de la malla 
            meshFrozen = true;
        }

        if (meshFrozen)
        {
            ExtractWallPoints();
            OnMeshProcessed?.Invoke(validPoints, validnormals); // pasar puntos y normales
        }
    }

    void ExtractWallPoints()
    {
       validPoints.Clear();

        Mesh mesh = meshFilter.sharedMesh;

        if (mesh == null)
        {
            Debug.LogError("El MeshFilter no contiene un Mesh válido.");
            return;
        }

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        for (int i = 0; i < vertices.Length; i++)
        {
            // Convertimos vértice y normal a espacio global
            Vector3 worldPoint = meshFilter.transform.TransformPoint(vertices[i]);
            Vector3 worldNormal = meshFilter.transform.TransformDirection(normals[i]).normalized;

            // Es pared si: 
            // la normal de y = 0 (o cerca) y el punto está a una altura mínima (para evitar suelo)
            bool isWall = Mathf.Abs(worldNormal.y) < normalThreshold && worldPoint.y > minGlobalHeight;

            if (isWall)
            {
                bool tooClose = false;

                // Evitar puntos demasiado juntos
                foreach (Vector3 point in validPoints)
                {
                    if (Vector3.Distance(point, worldPoint) < minDistanceBetweenPoints)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                {
                    validPoints.Add(worldPoint);
                    validnormals.Add(worldNormal);
                }
            }
        }
    }

    

    void OnDebugDraw()
    {
        foreach (Vector3 point in validPoints)
        {
            Instantiate(Resources.Load("DebugPointPrefab"), point, Quaternion.identity);
        }
    }


    public List<Vector3> GetValidPoints()
    {
        return new List<Vector3>(validPoints);
    }

    public Mesh GetMesh()
    {
        return meshFilter.sharedMesh;
    }

}