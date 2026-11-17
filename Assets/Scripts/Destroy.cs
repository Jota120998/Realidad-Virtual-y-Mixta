using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{   
    [Header("Configuración")]
    public float lifeTime = 3f;
    
    void OnEnable()
    {
        Destroy(gameObject, lifeTime);
    }

}
