using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    public LayerMask blockingLayers;
    public float checkRadius = 0.5f;

    [Header("Spawn Timing")]
    public float minSpawnTime = 8f;
    public float maxSpawnTime = 16f;

    [Header("Item Settings")]
    public float itemLifetime = 12f;
    public int maxActiveItems = 3;

    private Coroutine spawnCoroutine;
    private RoundData currentRound;

    // Relaciona cada punto con el item que ha aparecido allí.
    private Dictionary<Transform, GameObject> occupiedPoints =
        new Dictionary<Transform, GameObject>();

    public void StartSpawning(RoundData round)
    {
        // Evitar tener dos generadores activos.
        StopSpawning(true);

        currentRound = round;
        spawnCoroutine = StartCoroutine(SpawnItemsRoutine());
    }

    public void StopSpawning(bool destroyActiveItems = true)
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        if (destroyActiveItems)
        {
            foreach (GameObject item in occupiedPoints.Values)
            {
                if (item != null)
                    Destroy(item);
            }

            occupiedPoints.Clear();
        }
    }

    private IEnumerator SpawnItemsRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);

            yield return new WaitForSeconds(waitTime);

            // Comprobar si todavía hay espacio para otro item.
            CleanOccupiedPoints();

            if (occupiedPoints.Count >= maxActiveItems)
                continue;

            SpawnRandomItem();
        }
    }

    private void SpawnRandomItem()
    {
        if (currentRound == null)
            return;

        GameObject itemPrefab = GetRandomItem(currentRound);

        if (itemPrefab == null)
            return;

        List<Transform> availablePoints = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
                continue;

            // Comprobar si ya hay un item en ese punto.
            if (occupiedPoints.ContainsKey(point))
                continue;

            // Comprobar si hay enemigos u obstáculos ocupándolo.
            bool blocked = Physics.CheckSphere(
                point.position,
                checkRadius,
                blockingLayers,
                QueryTriggerInteraction.Ignore
            );

            if (!blocked)
                availablePoints.Add(point);
        }

        if (availablePoints.Count == 0)
        {
            Debug.Log("No hay puntos disponibles para generar power-ups.");
            return;
        }

        Transform selectedPoint =
            availablePoints[Random.Range(0, availablePoints.Count)];

        GameObject newItem = Instantiate(
            itemPrefab,
            selectedPoint.position,
            selectedPoint.rotation
        );

        occupiedPoints.Add(selectedPoint, newItem);

        // Destruir el item si nadie lo recoge a tiempo.
        Destroy(newItem, itemLifetime);

        Debug.Log("Power-up generado: " + newItem.name);
    }

    private GameObject GetRandomItem(RoundData round)
    {
        int totalWeight = 0;

        foreach (ItemSpawnInfo info in round.possibleItems)
        {
            if (info.itemPrefab != null && info.weight > 0)
                totalWeight += info.weight;
        }

        if (totalWeight <= 0)
            return null;

        int randomValue = Random.Range(0, totalWeight);

        foreach (ItemSpawnInfo info in round.possibleItems)
        {
            if (info.itemPrefab == null || info.weight <= 0)
                continue;

            randomValue -= info.weight;

            if (randomValue < 0)
                return info.itemPrefab;
        }

        return null;
    }

    private void CleanOccupiedPoints()
    {
        List<Transform> pointsToRemove = new List<Transform>();

        foreach (var pair in occupiedPoints)
        {
            // Unity devuelve null cuando el GameObject ha sido destruido.
            if (pair.Value == null)
                pointsToRemove.Add(pair.Key);
        }

        foreach (Transform point in pointsToRemove)
        {
            occupiedPoints.Remove(point);
        }
    }

    private void OnDisable()
    {
        StopSpawning(true);
    }
}