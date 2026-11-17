using UnityEngine;
using System;

public class Topo : MonoBehaviour
{
    public static event Action<int> OnTopoHit; // Evento de puntos
    public int points = 1;                     // Puntos que da al ser golpeado

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Hammer"))
        {
            // Dispara evento de puntos
            OnTopoHit?.Invoke(points);

            // Desactiva el topo para volver a la pool
            gameObject.SetActive(false);
        }
    }
}