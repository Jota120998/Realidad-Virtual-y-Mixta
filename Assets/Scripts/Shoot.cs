using System;
using UnityEngine;
using System.Collections;

public class XRGun : MonoBehaviour
{
    [Header("Referencias")]
    public Transform firePoint;
    public LineRenderer lineRenderer;
    //public Animator animator;
    public ParticleSystem particleSystm;
    public AudioSource shootAudio;

    [Header("Raycast Config")]
    public float shootDistance = 100f;

    [Header("Configuración")]
    public float shootColdown = 0.25f;
    public float lineDuration = 0.05f;

    private bool isShooting = false;

    public void Shoot()
    {
        if (isShooting) return;

        isShooting = true;

        //animator.SetTrigger("Shoot");

        Vector3 endPoint;

        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, shootDistance))
        {
            endPoint = hit.point;
            hit.collider.GetComponent<Dart>()?.Interact(hit.point);
        }
        else
        {
            endPoint = firePoint.position + firePoint.forward * shootDistance;
        }
        particleSystm.Play();
        shootAudio.Play();
        DrawLine(endPoint);

        StartCoroutine(ShootColdown(shootColdown));
    }

    void DrawLine(Vector3 endPoint)
    {
        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, firePoint.position);
        lineRenderer.SetPosition(1, endPoint);

        StartCoroutine(HideLine());
    }

    IEnumerator HideLine()
    {
        yield return new WaitForSeconds(lineDuration);
        lineRenderer.enabled = false;
    }

    public void FinishShooting()
    {
        isShooting = false;
    }

    private IEnumerator ShootColdown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        FinishShooting();
    }
}
