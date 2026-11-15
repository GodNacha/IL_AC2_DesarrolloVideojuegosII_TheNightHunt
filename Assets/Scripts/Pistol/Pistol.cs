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
    public float aimDistance = 100f;

    [Header("Camera Zoom")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float aimFOV = 45f;
    public float zoomSpeed = 8f;

    [Header("Disparo")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public int damage = 1;
    public AudioClip sfxShoot;
    public AudioClip sfxNoAmmo;
    public AudioClip sfxReload;
    public AudioSource pistolSource;

    private int baseDamage;

    [Header("Efectos")]
    public Transform VFXSpawn;
    public ParticleSystem VFXShoot;

    [Header("Municion")]
    public int bullets = 12;
    public int maxBullets = 12;
    public int chargers = 2;
    public float delayReload = 1.5f;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    [Header("UI")]
    public TextMeshProUGUI currentBulletText;
    public TextMeshProUGUI currentChargerText;
    public GameObject puntero;

    public bool canShoot;

    private bool isAiming;
    private bool previousAiming = false;
    private bool isReloading;

    private Pause pause;
    private GameManager gameManager;

    private void Awake()
    {
        baseDamage = damage;

        cameraShake = FindAnyObjectByType<CameraShake>();
        pause = FindAnyObjectByType<Pause>();
        gameManager = FindAnyObjectByType<GameManager>();
        puntero = GameObject.Find("Puntero");
    }

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
        // Apuntar con el botón derecho.
        isAiming = Input.GetMouseButton(1);

        // Detectar cambios en el estado de apuntado.
        if (isAiming != previousAiming)
        {
            puntero.SetActive(!isAiming);
            previousAiming = isAiming;
        }

        // Mover la pistola hacia la posición correspondiente.
        Transform target = isAiming ? ZoomPostion : defaultPostion;

        transform.position = Vector3.Lerp(
            transform.position,
            target.position,
            aimSpeed * Time.deltaTime
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            target.rotation,
            aimSpeed * Time.deltaTime
        );

        // Aplicar zoom a la cámara.
        float targetFOV = isAiming ? aimFOV : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            zoomSpeed * Time.deltaTime
        );

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
        if (!canShoot || isReloading || pause.isPaused || gameManager.gameOver)
            return;

        if (bullets <= 0)
        {
            Debug.Log("No Ammo, you can't shoot");

            pistolSource.PlayOneShot(sfxNoAmmo);
            return;
        }

        bullets--;
        UpdateAmmoUI();

        Instantiate(VFXShoot, VFXSpawn.position, VFXSpawn.rotation);
        SpawnBullet();

        if (cameraShake != null)
        {
            cameraShake.ShakePistol();
        }

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

        bulletScript.damage = damage; //Define el daño de la bala

        if (bulletScript != null)
        {
            bulletScript.SetDirection(shootDirection);
        }
        else
        {
            Debug.LogError("El prefab no tiene el componente Bullet.");
        }
    }

    public void AddAmmo(int amount)
    {
        chargers += amount;

        UpdateAmmoUI();

        Debug.Log("Has conseguido " + amount + " cartucho(s) extra.");
    }


    public void StartReload()
    {
        if (bullets == maxBullets)
        {
            Debug.Log("Munición al máximo.");
            return;
        }
       
        if (isReloading)
            return;

        if (chargers <= 0)
        {
            Debug.Log("No tienes cargadores disponibles.");
            return;
        }

        if (!canShoot || gameManager.gameOver)
        {
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
        bullets = maxBullets;
        UpdateAmmoUI();

        canShoot = true;
        isReloading = false;
    }

    public void UpdateAmmoUI()
    {
        currentBulletText.text = bullets.ToString();
        currentChargerText.text = chargers.ToString();
    }

    public void SetExtraDamage(int bonus)
    {
        damage = baseDamage + bonus;

        Debug.Log("Daño actual de la pistola: " + damage);
    }

    public void RemoveExtraDamage()
    {
        damage = baseDamage;

        Debug.Log("Daño restaurado: " + damage);
    }
}
