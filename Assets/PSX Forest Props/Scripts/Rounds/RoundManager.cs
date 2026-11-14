
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoundManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Rounds")]
    public RoundData[] rounds;
    public float delayStartSpawning = 2f;

    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    public float spawnCheckRadius = 1f;
    public LayerMask spawnBlockingLayers;

    [Header("Spawn Retry")]
    public float spawnRetryInterval = 1f;

    [Header("Item Spawner")]
    public ItemSpawner itemSpawner;

    [Header("UI")]
    public TextMeshProUGUI roundText;
    public GameObject roundPanel;
    public float timeTextRound = 3f;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    private int currentRoundIndex = -1;
    private int aliveEnemies = 0;
    private bool roundInProgress;

    private readonly List<EnemyController> activeEnemies = new();

    private bool isSpawning;

    private GameManager gameManager;

    private void Start()
    {
        roundPanel.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();

    }

    public void StopAllCoroutinesOnRoad()
    {
        StopAllCoroutines();
    }

    public void StartGame()
    {
        if (currentRoundIndex == -1)
        {
            StartCoroutine(StartNextRound());
        }
    }

    private IEnumerator StartNextRound()
    {
        currentRoundIndex++;

        if (currentRoundIndex >= rounds.Length)
        {
            Debug.Log("¡Todas las rondas completadas!");
            yield break;
        }

        roundPanel.SetActive(true);

        StartCoroutine(ShowRoundText(currentRoundIndex + 1));

        RoundData round = rounds[currentRoundIndex];

        Debug.Log("Comenzando ronda " + (currentRoundIndex + 1));

        if (cameraShake != null)
            cameraShake.ShakeRound();

        yield return new WaitForSeconds(delayStartSpawning);

        roundInProgress = true;

        itemSpawner.StartSpawning(round);

        isSpawning = true;

        // Crear una lista con todos los enemigos de la ronda.
        List<EnemyData> enemiesToSpawn = new List<EnemyData>();

        foreach (EnemySpawnInfo enemyInfo in round.enemies)
        {
            for (int i = 0; i < enemyInfo.amount; i++)
            {
                enemiesToSpawn.Add(enemyInfo.enemyData);
            }
        }

        // Mezclar aleatoriamente el orden de los enemigos.
        ShuffleEnemies(enemiesToSpawn);

        // Generar los enemigos en el orden aleatorio.
        foreach (EnemyData enemyData in enemiesToSpawn)
        {
            if (enemyData == null || enemyData.enemyPrefab == null)
            {
                Debug.LogError("Hay un EnemyData o prefab sin asignar.");
                continue;
            }

            Transform spawn = null;

            // Buscar hasta encontrar un punto libre.
            while (spawn == null)
            {
                spawn = GetAvailableSpawnPoint();

                if (spawn == null)
                {
                    Debug.Log("No hay puntos libres. Reintentando spawn...");
                    yield return new WaitForSeconds(spawnRetryInterval);
                }
            }

            // Crear enemigo.
            GameObject enemyObject = Instantiate(
                enemyData.enemyPrefab,
                spawn.position,
                spawn.rotation
            );

            EnemyController enemy =
                enemyObject.GetComponent<EnemyController>();

            enemy.Initialize(enemyData, player);

            enemy.OnEnemyDeath += OnEnemyDeath;

            activeEnemies.Add(enemy);
            aliveEnemies++;

            // Esperar antes de generar el siguiente enemigo.
            yield return new WaitForSeconds(round.spawnInterval);
        }

        isSpawning = false;

        // Si todos los enemigos ya murieron, terminar la ronda.
        if (aliveEnemies == 0)
            StartCoroutine(EndRound());
    }

    private void ShuffleEnemies(List<EnemyData> enemies)
    {
        // Algoritmo Fisher-Yates para mezclar la lista.
        for (int i = enemies.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            EnemyData temp = enemies[i];
            enemies[i] = enemies[randomIndex];
            enemies[randomIndex] = temp;
        }
    }

    private Transform GetAvailableSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (point == null)
                continue;

            bool blocked = Physics.CheckSphere(
                point.position,
                spawnCheckRadius,
                spawnBlockingLayers,
                QueryTriggerInteraction.Ignore
            );

            if (!blocked)
                availablePoints.Add(point);
        }

        if (availablePoints.Count == 0)
            return null;

        return availablePoints[Random.Range(0, availablePoints.Count)];
    }

    private void OnEnemyDeath(EnemyController enemy)
    {
        activeEnemies.Remove(enemy);
        aliveEnemies--;

        if (aliveEnemies <= 0 && !isSpawning && roundInProgress)
        {
            StartCoroutine(EndRound());
        }
    }

    private IEnumerator EndRound()
    {
        if (!roundInProgress)
            yield break;

        roundInProgress = false;

        itemSpawner.StopSpawning(true);

        Debug.Log("Ronda completada.");

        yield return new WaitForSeconds(
            rounds[currentRoundIndex].timeBeforeNextRound
        );

        StartCoroutine(StartNextRound());
    }

    private IEnumerator ShowRoundText(int roundNumber)
    {
        roundText.text = "ROUND " + roundNumber;

        roundPanel.SetActive(true);

        yield return new WaitForSeconds(timeTextRound);

        roundPanel.SetActive(false);
    }
}
