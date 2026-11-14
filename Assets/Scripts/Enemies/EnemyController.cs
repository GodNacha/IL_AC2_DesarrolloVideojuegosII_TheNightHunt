using UnityEngine;
using System;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour, IDamageable
{
    private int currentHealth;
    private float moveSpeed;
    private int pointsReward;
    private Transform player;

    private Rigidbody rb;

    public Action<EnemyController> OnEnemyDeath;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip deadSFX;
    public AudioClip StepsSFX;
    public AudioClip voiceSFX;

    private GameManager gameManager;

    public void Initialize(EnemyData data, Transform playerTarget)
    {
        currentHealth = data.maxHealth;
        moveSpeed = data.moveSpeed;
        player = playerTarget;
        pointsReward = data.pointsReward;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        gameManager = FindAnyObjectByType<GameManager>();
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (gameManager.gameOver) return;

        Vector3 direction = player.position - rb.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();

            rb.MovePosition(
                rb.position + direction * moveSpeed * Time.fixedDeltaTime
            );

            rb.MoveRotation(Quaternion.LookRotation(direction));
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Daño recibido:" + damage);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //Agregar puntos al ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddPoints(pointsReward);
        }

        OnEnemyDeath?.Invoke(this);

        if (deadSFX != null)
        {
            audioSource.PlayOneShot(deadSFX);

        }

        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.GameOver();
        }
    }
}