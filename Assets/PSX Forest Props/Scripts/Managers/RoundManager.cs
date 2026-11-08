using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering.LookDev;
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

    private void Start()
    {
        roundPanel.SetActive(false);
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

        // Actualizar texto de la ronda
        StartCoroutine(ShowRoundText(currentRoundIndex + 1));

        RoundData round = rounds[currentRoundIndex];

        Debug.Log("Comenzando ronda " + (currentRoundIndex + 1));

        if (cameraShake != null)
            cameraShake.Shake();

        yield return new WaitForSeconds(delayStartSpawning);

        roundInProgress = true;

        isSpawning = true;

        foreach (EnemySpawnInfo enemyInfo in round.enemies)
        {
            for (int i = 0; i < enemyInfo.amount; i++)
            {
                Transform spawn = GetAvailableSpawnPoint();

                if (spawn == null)
                {
                    Debug.LogWarning("No se encontró un punto de spawn libre.");
                    continue;
                }

                GameObject enemyObject = Instantiate(
                    enemyInfo.enemyData.enemyPrefab,
                    spawn.position,
                    spawn.rotation
                );

                EnemyController enemy =
                    enemyObject.GetComponent<EnemyController>();

                enemy.Initialize(enemyInfo.enemyData, player);

                enemy.OnEnemyDeath += OnEnemyDeath;

                activeEnemies.Add(enemy);
                aliveEnemies++;

                yield return new WaitForSeconds(round.spawnInterval);
            }
        }

        isSpawning = false;

        // Si no se han podido crear enemigos, la ronda termina.
        if (aliveEnemies == 0)
            StartCoroutine(EndRound());
    }

    private Transform GetAvailableSpawnPoint()
    {
        List<Transform> availablePoints = new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
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

        Debug.Log("Ronda completada.");

        yield return new WaitForSeconds(
            rounds[currentRoundIndex].timeBeforeNextRound
        );

        StartCoroutine(StartNextRound());
    }

    private IEnumerator ShowRoundText(int roundNumber)
    {
        roundText.text = "ROUND " + roundNumber;

        // Mostrar texto
        roundPanel.SetActive(true);

        // Mantener visible
        yield return new WaitForSeconds(timeTextRound);     

         roundPanel.SetActive(false);
    }
}