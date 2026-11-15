using UnityEngine;
using System;
using UnityEngine.Rendering;
using System.Collections;

public class EnemyController : MonoBehaviour, IDamageable
{
    private int currentHealth;
    private float moveSpeed;
    private int pointsReward;
    private Transform player;

    private Rigidbody rb;

    public Action<EnemyController> OnEnemyDeath;

    [Header("Other Settings")]
    public float delayDead = 2.5f;

    [Header("Audio Settings")]
    public AudioSource audioSource;

    [Header("Audio Clips Genereic")]
    public AudioClip stepsSFX;
    public AudioClip breathSFX;
    public AudioClip deadSFX;
    public AudioClip hitSFX;
    public AudioClip bulletImpactZombieSfx;

    [Header("Audio Clips Screams")]
    public AudioClip screamsSFX;


    [Header("VFX Settings")]
    public ParticleSystem vfxBlood;
    public Transform vfxSpawn;

    private GameManager gameManager;
    private Animator anim;
    private bool invulnerable = false;

    private bool stopSFX = false;

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
        anim = GetComponent<Animator>();
        stopSFX = false;
    }

    private void Start()
    {
        audioSource.clip = breathSFX;
        audioSource.loop = true;
        audioSource.Play();

        audioSource.PlayOneShot(screamsSFX);
    }

    private void FixedUpdate()
    {
        if (player == null) return;

        if (gameManager.gameOver || invulnerable) return;

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

    private void Update()
    {
        if (gameManager.gameOver && !stopSFX)
        {
            stopSFX = true;
            audioSource.Pause();
        }
    }

    public void TakeDamage(int damage)
    {
        if (invulnerable) return;

        
        audioSource.PlayOneShot(bulletImpactZombieSfx);
        currentHealth -= damage;

        Debug.Log("Daño recibido:" + damage);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            audioSource.PlayOneShot(hitSFX);
        }
    }

    private void Die()
    {
        invulnerable = true;

        audioSource.Pause(); //Ver si esto es un problema

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

        if (vfxBlood != null)
        {
            Instantiate(vfxBlood, vfxSpawn.position, vfxSpawn.rotation);

        }

        StartCoroutine(DelayDead());
    }

    private IEnumerator DelayDead()
    {
        anim.SetTrigger("Dead");

        yield return new WaitForSeconds(delayDead);

        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.GameOver();
            audioSource.Pause();
        }
    }
}