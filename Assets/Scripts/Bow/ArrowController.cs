using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private GameObject midPointVisual, spawnPoint, arrowPrefab;

    [SerializeField] private float arrowMaxSpeed = 10;

    void OnEnable()
    {
        BowStringController.OnBowPulled += PullArrow;
        BowStringController.OnBowRelease += ReleaseArrow;
    }

    void OnDisable()
    {
        BowStringController.OnBowPulled -= PullArrow;
        BowStringController.OnBowRelease-= ReleaseArrow;
    }
    private void PullArrow()
    {
        midPointVisual.SetActive(true);
    }

    private void ReleaseArrow(float strenght)
    {   
        
        midPointVisual.SetActive(false);

        GameObject arrow = Instantiate(arrowPrefab);

        arrow.transform.position = spawnPoint.transform.position;
        arrow.transform.rotation = midPointVisual.transform.rotation;

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(midPointVisual.transform.forward * strenght * arrowMaxSpeed,ForceMode.Impulse);
    }
}
