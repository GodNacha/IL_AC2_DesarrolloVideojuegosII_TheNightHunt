
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
    public int ammoNextRound = 2;

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
    public TextMeshPro roundPassText;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    [Header("Audio")]
    public AudioSource roundSource;
    public AudioClip startRound;
    public AudioClip passRound;
    public AudioClip endAllRounds;

    private int currentRoundIndex = -1;
    private int aliveEnemies = 0;
    private bool roundInProgress;

    private readonly List<EnemyController> activeEnemies = new();

    private bool isSpawning;
    private bool isFinalRound;

    private GameManager gameManager;
    private Pistol pistol;
    private MusicManager musicManager;

    private void Start()
    {
        roundPanel.SetActive(false);
        gameManager = FindAnyObjectByType<GameManager>();
        pistol = FindAnyObjectByType<Pistol>();
        roundPassText.text = null;
        roundSource = GetComponent<AudioSource>();
        musicManager = FindAnyObjectByType<MusicManager>();
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

        // Detectar si esta es la última ronda.
        isFinalRound = currentRoundIndex == rounds.Length - 1;

        roundPanel.SetActive(true);

        roundSource.PlayOneShot(startRound); //Sonido de inicio de ronda

        if (isFinalRound)
        {
            StartCoroutine(ShowRoundText("FINAL ROUND"));
            Debug.Log("¡COMIENZA LA RONDA FINAL!");
        }
        else
        {
            StartCoroutine(ShowRoundText("ROUND " + (currentRoundIndex + 1)));
        }

        RoundData round = rounds[currentRoundIndex];

        Debug.Log("Comenzando ronda " + (currentRoundIndex + 1));

        if (cameraShake != null)
            cameraShake.ShakeRound();

        yield return new WaitForSeconds(delayStartSpawning);
         
        musicManager.CallFadeOutMusic();  //Colocar aquí el inicio de la música de ronda

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

        musicManager.CallFadeInMusic(); //Fade In música al pasar una ronda, se acaba la música

        Debug.Log("Ronda completada.");

        // Comprobar si se ha completado la ronda final.
        if (isFinalRound)
        {
            Debug.Log("¡RONDA FINAL COMPLETADA!");

            roundPassText.text = ("YOU KILL THEM ALL!");

            roundSource.PlayOneShot(endAllRounds); //Sonido de se completaron todas las rondas

            gameManager.Win();

            yield break;
        }

        roundPassText.text = ("HORDE ANNIHILATED");

        roundSource.PlayOneShot(passRound); //Sonido se completó la ronda

        // Si no es la ronda final, continuar normalmente.
        yield return new WaitForSeconds(
            rounds[currentRoundIndex].timeBeforeNextRound
        );

        roundPassText.text = null;

        pistol.AddAmmo(ammoNextRound);

        StartCoroutine(StartNextRound());
    }

    private IEnumerator ShowRoundText(string message)
    {
        roundText.text = message;

        roundPanel.SetActive(true);

        yield return new WaitForSeconds(timeTextRound);

        roundPanel.SetActive(false);
    }
}
