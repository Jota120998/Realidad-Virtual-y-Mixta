using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class BowStringController : MonoBehaviour
{
    [Header("Line Renderer")]
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Bow Points")]
    [SerializeField] private Transform StringLocalVisualization;

    [SerializeField] private Transform midPointGrabObject;
    [SerializeField] private Transform midPointVisual;
    [SerializeField] private Transform startMid;
    [SerializeField] private Transform endMid;
    [SerializeField] private GameObject grabableObj;
    //Creamos eventos
    public static event Action OnBowPulled;
    public static event Action<float> OnBowRelease;

    public float PullAmount{get; private set;} = 0.0f;

    private XRGrabInteractable interactable;
    private Transform interactor;


    private void Awake()
    {

        interactable = midPointGrabObject.GetComponent<XRGrabInteractable>();
        
    }
    
    private void Start()
    {   

        //Oimos a los eventos de Select para este Script del controlador
        interactable.selectEntered.AddListener(PullStringInteraction);
        interactable.selectExited.AddListener(ReleaseString);
    }

    private void PullStringInteraction(SelectEnterEventArgs args)
    {   
        Debug.Log(args.interactorObject.transform.name);
        interactor = args.interactorObject.transform; //Asociamos el transform del mando al interactor(transform) para tener una referencia
        OnBowPulled?.Invoke();
    }
    private void ReleaseString(SelectExitEventArgs args)
    {   
        //Dispara evento OnBowRelease
        OnBowRelease?.Invoke(PullAmount);
       
        interactor = null;
        midPointGrabObject.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        PullAmount = 0f;
        UpdateString();    
    }

    private void Update()
    {
        //Si no hay nada cogido
        if(interactor == null) return;

        Vector3 pullPosition = interactor.position;//por si se actualiza tener un valor fijo
        PullAmount = CalculatePull(pullPosition);
        UpdateString();    
    }

    private float CalculatePull(Vector3 pullPosition) //
    {   
        Vector3 pullDirection = pullPosition - startMid.position;
        Vector3 pullNormal = endMid.position - startMid.position;
        float maxLength = pullNormal.magnitude;

        pullNormal.Normalize(); //  Normalizamos para que sea una dirección
        float pullValue = Vector3.Dot(pullDirection,pullNormal) / maxLength;
        return Mathf.Clamp(pullValue,0,1);
    }

    private void UpdateString()
    {
        if (lineRenderer == null) return;

        // Interpolamos el punto medio entre startMid y endMid según PullAmount y lo movemos ya que es un transform de la escena
        midPointVisual.position = Vector3.Lerp(startMid.position, endMid.position, PullAmount);

        // Actualizamos el LineRenderer
        lineRenderer.SetPosition(1, StringLocalVisualization.InverseTransformPoint(midPointVisual.position));     // punto medio que se mueve referenciado a las cuerdas locales
        
    }

    public void EnableDisablePull(bool condition)
    {
        grabableObj.SetActive(condition);
    }

}