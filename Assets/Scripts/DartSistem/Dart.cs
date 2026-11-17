using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Dart : MonoBehaviour
{
    [SerializeField] private int points = 1;
    [Header("Break FX")]
    [SerializeField] private ParticleSystem breakParticlesPrefab;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] [Range(0f, 1f)] private float breakSoundVolume = 1f;
    [SerializeField] private int breakParticleCount = 18;
    [SerializeField] private float breakParticleLifetime = 0.45f;
    [SerializeField] private float breakParticleSpeed = 2.4f;
    [SerializeField] private float breakParticleSize = 0.08f;
    [SerializeField] private Color breakParticleStartColor = new Color(1f, 0.87f, 0.35f, 1f);
    [SerializeField] private Color breakParticleEndColor = new Color(1f, 0.24f, 0.08f, 0f);

    public static event Action<int,GameObject> OnDartHit;
    private bool isBroken;

    void OnEnable()
    {
        isBroken = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Arrow"))
        {
            Break(GetHitPoint(other.transform.position), other.gameObject);
        }
    }

    public void Interact()
    {
        Break(transform.position);
    }

    public void Interact(Vector3 hitPoint)
    {
        Break(hitPoint);
    }

    private void Break(Vector3 hitPoint, GameObject projectileToDestroy = null)
    {
        if (isBroken) return;

        isBroken = true;
        PlayBreakSound(hitPoint);
        SpawnBreakParticles(hitPoint);
        OnDartHit?.Invoke(points,gameObject);
        gameObject.SetActive(false);

        if (projectileToDestroy != null)
            Destroy(projectileToDestroy);
    }

    private Vector3 GetHitPoint(Vector3 sourcePosition)
    {
        Collider dartCollider = GetComponent<Collider>();
        return dartCollider != null ? dartCollider.ClosestPoint(sourcePosition) : transform.position;
    }

    private void SpawnBreakParticles(Vector3 hitPoint)
    {
        ParticleSystem particles = breakParticlesPrefab != null
            ? Instantiate(breakParticlesPrefab, hitPoint, Quaternion.identity)
            : CreateDefaultBreakParticles(hitPoint);

        var main = particles.main;
        main.loop = false;
        main.stopAction = ParticleSystemStopAction.Destroy;
        particles.Play();
    }

    private void PlayBreakSound(Vector3 hitPoint)
    {
        if (breakSound == null) return;

        AudioSource.PlayClipAtPoint(breakSound ,hitPoint, breakSoundVolume);
    }

    private ParticleSystem CreateDefaultBreakParticles(Vector3 hitPoint)
    {
        GameObject particleObject = new GameObject("DartBreakParticles");
        particleObject.transform.SetPositionAndRotation(hitPoint, Quaternion.identity);

        ParticleSystem particles = particleObject.AddComponent<ParticleSystem>();

        var main = particles.main;
        main.duration = 0.2f;
        main.loop = false;
        main.startLifetime = breakParticleLifetime;
        main.startSpeed = breakParticleSpeed;
        main.startSize = breakParticleSize;
        main.startColor = breakParticleStartColor;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = breakParticleCount;
        main.gravityModifier = 0.15f;

        var emission = particles.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, (short)breakParticleCount) });

        var shape = particles.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.08f;

        var colorOverLifetime = particles.colorOverLifetime;
        colorOverLifetime.enabled = true;

        Gradient fadeGradient = new Gradient();
        fadeGradient.SetKeys(
            new[]
            {
                new GradientColorKey(breakParticleStartColor, 0f),
                new GradientColorKey(new Color(breakParticleEndColor.r, breakParticleEndColor.g, breakParticleEndColor.b, 1f), 1f),
            },
            new[]
            {
                new GradientAlphaKey(breakParticleStartColor.a, 0f),
                new GradientAlphaKey(breakParticleEndColor.a, 1f),
            });
        colorOverLifetime.color = new ParticleSystem.MinMaxGradient(fadeGradient);

        var sizeOverLifetime = particles.sizeOverLifetime;
        sizeOverLifetime.enabled = true;
        sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(
            1f,
            AnimationCurve.EaseInOut(0f, 1f, 1f, 0f));

        return particles;
    }
}
