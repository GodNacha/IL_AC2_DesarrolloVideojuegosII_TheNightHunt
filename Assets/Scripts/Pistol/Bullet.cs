using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage;
    public float velocity = 20f;
    public float lifeTime = 5f;

    [Header("Collision Settings")]
    public float detectionRadius = 0.1f;
    public LayerMask hitLayers = ~0;

    private Rigidbody rb;
    private bool hasHit;

    public ParticleSystem destroyVFX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    public void SetDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.001f)
        {
            Debug.LogError("La dirección de la bala es cero.");
            return;
        }

        rb.linearVelocity = direction.normalized * velocity;

        Debug.Log($"Velocidad aplicada: {rb.linearVelocity}");
    }

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void FixedUpdate()
    {
        if (hasHit) return;

        Vector3 movement = rb.linearVelocity * Time.fixedDeltaTime;

        float distance = movement.magnitude;

        if (distance <= 0f) return;

        Vector3 direction = movement.normalized;

        // Comprobar si hay algo en el recorrido de la bala.
        if (Physics.SphereCast(
            rb.position,
            detectionRadius,
            direction,
            out RaycastHit hit,
            distance,
            hitLayers,
            QueryTriggerInteraction.Collide))
        {
            HandleImpact(hit.collider);
        }
    }

    private void HandleImpact(Collider other)
    {
        if (hasHit) return;

        hasHit = true;

        // Comprobar si es un objeto especial
        IShootable shootable = other.GetComponentInParent<IShootable>();

        if (shootable != null)
        {
            shootable.OnShot();
        }
        else
        {
            // Si no es especial, comprobar si recibe daño
            IDamageable damageable = other.GetComponentInParent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }

        if (destroyVFX != null)
        {
            Instantiate(destroyVFX, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }
}


