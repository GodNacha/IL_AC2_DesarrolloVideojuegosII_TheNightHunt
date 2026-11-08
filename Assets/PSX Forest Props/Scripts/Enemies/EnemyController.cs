using UnityEngine;
using System;
using UnityEngine.Rendering;

public class EnemyController : MonoBehaviour, IDamageable
{
    private int currentHealth;
    private float moveSpeed;
    private Transform player;

    public Action<EnemyController> OnEnemyDeath;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip deadSFX;
    public AudioClip StepsSFX;
    public AudioClip voiceSFX;

    public void Initialize(EnemyData data, Transform playerTarget)
    {
        currentHealth = data.maxHealth;
        moveSpeed = data.moveSpeed;
        player = playerTarget;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (player == null) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
            transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnEnemyDeath?.Invoke(this);

        if (deadSFX != null)
        {
            audioSource.PlayOneShot(deadSFX);

        }

        Destroy(gameObject);
    }
}