using UnityEngine;
using System.Collections;
using TMPro;

public class Pistol : MonoBehaviour
{
    [Header("Posiciones")]
    public Transform defaultPostion;
    public Transform ZoomPostion;
    public float aimSpeed = 12f;
    public Transform cameraPosition;
    public Camera playerCamera;
    public float aimDistance = 100f;

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public AudioClip sfxShoot;
    public AudioClip sfxReload;
    public AudioSource pistolSource;

    [Header("Efectos")]
    public Transform VFXSpawn;
    public ParticleSystem VFXShoot;

    [Header("Municion")]
    public int bullets = 12;
    public int maxBullets = 12;
    public int chargers = 2;
    public float delayReload = 1.5f;

    [Header("UI")]
    public TextMeshProUGUI currentBulletText;
    public TextMeshProUGUI currentChargerText;

    public bool canShoot;

    private bool isAiming;
    private bool isReloading;

    void Start()
    {
        //Se coloca en la posición default
        transform.SetPositionAndRotation(defaultPostion.position, defaultPostion.rotation);

        currentBulletText.text = bullets.ToString();
        currentChargerText.text = chargers.ToString();

        if (pistolSource == null)
        {
            Debug.Log("Audiosource de Pistola no asignado");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Apuntar con el boton derecho. CAMBIAR CON EL SISTEMA NUEVO DE INPUTS
        isAiming = Input.GetMouseButton(1);

        // Mover la pistola hacia la posicion correspondiente.
        Transform target = isAiming ? ZoomPostion : defaultPostion;

        transform.position = Vector3.Lerp(transform.position, target.position, aimSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Slerp(cameraPosition.rotation, cameraPosition.rotation, aimSpeed * Time.deltaTime);     

        // Disparar con el boton izquierdo.
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }

        // Recargar con R.
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }


    public void Shoot()
    {
        if (!canShoot || isReloading)
            return;

        if (bullets <= 0)
        {
            Debug.Log("No Ammo, you can't shoot");
            return;
        }

        bullets--;
        currentBulletText.text = bullets.ToString();

        Instantiate(VFXShoot, VFXSpawn.position, VFXSpawn.rotation);
        SpawnBullet();

        pistolSource.PlayOneShot(sfxShoot);
    }

    public void SpawnBullet()
    {
        if (bulletPrefab == null || bulletSpawn == null || playerCamera == null)
        {
            Debug.LogWarning("Faltan referencias para disparar.");
            return;
        }

        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        Vector3 targetPoint;

        if (Physics.Raycast(ray, out RaycastHit hit, aimDistance))
            targetPoint = hit.point;
        else
            targetPoint = ray.GetPoint(aimDistance);

        Vector3 shootDirection =
            (targetPoint - bulletSpawn.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            Quaternion.LookRotation(shootDirection)
        );

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
        }
        else
        {
            Debug.LogError("El prefab no tiene el componente Bullet.");
        }
    }


    public void StartReload()
    {
        if (bullets == maxBullets)
        {
            Debug.Log("No tienes cargadores disponibles.");
            return;
        }
       
        if (isReloading)
            return;

        if (chargers <= 0)
        {
            Debug.Log("No tienes cargadores disponibles.");
            return;
        }

        StartCoroutine(DelayReload());
    }

    IEnumerator DelayReload()
    {
        pistolSource.PlayOneShot(sfxReload);
        isReloading = true;
        canShoot = false;

        yield return new WaitForSeconds(delayReload);

        chargers--;
        currentChargerText.text = chargers.ToString();
        bullets = maxBullets;
        currentBulletText.text = bullets.ToString();

        canShoot = true;
        isReloading = false;
    }
}
