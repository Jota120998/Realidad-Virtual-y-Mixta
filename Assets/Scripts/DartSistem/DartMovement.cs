using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private bool xMovement = true;   // Movimiento horizontal
    [SerializeField] private bool yMovement = false;  // Movimiento vertical
    [SerializeField] private float velocity = 1f;     // Velocidad del movimiento
    [SerializeField] private float amplitude = 1f;    // Distancia máxima de oscilación

    private Vector3 startPoint;
    private float moveTimer;

    void Awake()
    {
        // Guardamos posición inicial
        startPoint = transform.position;
    }

    void OnEnable()
    {
        // Reiniciamos la posición y el temporizador
        transform.position = startPoint;
        moveTimer = 0f;
    }

    void Update()
    {
        MoveDart();
    }

    private void MoveDart()
    {
        moveTimer += Time.deltaTime;

        // Calculamos desplazamiento con Sin para oscilación
        float offsetX = xMovement ? Mathf.Cos(moveTimer * velocity) * amplitude : 0f;
        float offsetY = yMovement ? Mathf.Sin(moveTimer * velocity) * amplitude : 0f;

        transform.position = startPoint + new Vector3(offsetX, offsetY, 0f);
    }

    public void setDificulty(int velocityD)
    {
        velocity = velocityD;
    }
}