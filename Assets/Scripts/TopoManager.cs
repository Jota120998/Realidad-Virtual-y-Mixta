using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopoManager : MonoBehaviour
{
    [Header("Topos")]
    public List<GameObject> allTopos;

    [Header("Tiempos")]
    public float minVisibleTime = 0.5f;
    public float maxVisibleTime = 2f;
    public float minSpawnInterval = 0.2f;
    public float maxSpawnInterval = 0.5f;

    private Coroutine spawnCoroutine;

    // Inicia el spawn
    public void StartSpawning()
    {
        foreach (var topo in allTopos)
            topo.SetActive(false);

        spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        foreach (var topo in allTopos)
            topo.SetActive(false);
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            GameObject topo = GetRandomInactiveTopo();
            if (topo != null)
            {
                topo.SetActive(true);
                float showTime = Random.Range(minVisibleTime, maxVisibleTime);
                StartCoroutine(HideTopoAfterTime(topo, showTime));
            }

            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));
        }
    }

    private GameObject GetRandomInactiveTopo()
    {
        List<GameObject> inactiveTopos = new List<GameObject>();
        foreach(var topo in allTopos)
        {
            if(!topo.activeInHierarchy)
                inactiveTopos.Add(topo);
        }

        if(inactiveTopos.Count == 0) return null;

        int index = Random.Range(0, inactiveTopos.Count);
        return inactiveTopos[index];
    }

    private IEnumerator HideTopoAfterTime(GameObject topo, float time)
    {
        yield return new WaitForSeconds(time);
        if(topo != null && topo.activeInHierarchy)
            topo.SetActive(false);
    }
}