using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 5;
    public float velocity = 20f;

    private Rigidbody rb;
    private Vector3 direction;

    public ParticleSystem VFXdestroy;

    public bool hasHit;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
        // rb.linearVelocity = direction * velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Aqui puedes aplicar dano al objeto impactado.
        if (VFXdestroy != null)
        {
            Instantiate(VFXdestroy, transform.position, transform.rotation);
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Tambien funciona con colliders configurados como Trigger.
        Destroy(gameObject);
    }

    private void Hit()
    {
        if (hasHit) return;

        hasHit = true;
        Destroy(gameObject);
    }
}
