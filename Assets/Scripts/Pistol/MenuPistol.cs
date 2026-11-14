
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPistol : MonoBehaviour
{
    [Header("Posiciones")]
    public Transform defaultPostion;
    public Transform ZoomPostion;
    public float aimSpeed = 12f;
    public Transform cameraPosition;

    [Header("Camera Zoom")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float aimFOV = 45f;
    public float zoomSpeed = 8f;

    [Header("Disparo")]
    public AudioClip sfxShoot;
    public AudioSource pistolSource;
    public float delayAfterShootUI = 1.5f;

    [Header("Efectos")]
    public Transform VFXSpawn;
    public ParticleSystem VFXShoot;

    [Header("Camera Shake")]
    public CameraShake cameraShake;

    [Header("Menu Interaction")]
    public bool canShoot = true;

    private bool isAiming;
    private Button currentTargetButton;
    private Button previousTargetButton;

    private void Awake()
    {
        cameraShake = FindAnyObjectByType<CameraShake>();
    }

    private void Start()
    {
        transform.SetPositionAndRotation(
            defaultPostion.position,
            defaultPostion.rotation
        );

        if (pistolSource == null)
        {
            Debug.LogWarning("AudioSource de Pistola no asignado.");
        }

        if (EventSystem.current == null)
        {
            Debug.LogError("No hay un EventSystem en la escena.");
        }
    }

    private void Update()
    {
        // Apuntar con botón derecho.
        isAiming = Input.GetMouseButton(1);

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

        // Zoom de cámara.
        float targetFOV = isAiming ? aimFOV : normalFOV;

        playerCamera.fieldOfView = Mathf.Lerp(
            playerCamera.fieldOfView,
            targetFOV,
            zoomSpeed * Time.deltaTime
        );

        // Detectar el botón que está bajo la mira.
        UpdateButtonFeedback();

        // Disparar con botón izquierdo.
        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    private Button GetButtonUnderCrosshair()
    {
        if (EventSystem.current == null)
            return null;

        PointerEventData pointerData = new PointerEventData(
            EventSystem.current
        );

        pointerData.position = new Vector2(
            Screen.width / 2f,
            Screen.height / 2f
        );

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            Button button = result.gameObject.GetComponentInParent<Button>();

            if (button != null && button.interactable)
            {
                return button;
            }
        }

        return null;
    }

    public void Shoot()
    {
        if (!canShoot)
            return;

        // Reproducir efecto visual.
        if (VFXShoot != null && VFXSpawn != null)
        {
            Instantiate(
                VFXShoot,
                VFXSpawn.position,
                VFXSpawn.rotation
            );
        }

        // Reproducir sonido.
        if (pistolSource != null && sfxShoot != null)
        {
            pistolSource.PlayOneShot(sfxShoot);
        }

        // Shake de cámara.
        if (cameraShake != null)
        {
            cameraShake.ShakePistol();
        }

        // Ejecutar el botón que está bajo la mira.
        if (currentTargetButton != null)
        {
            Debug.Log("Botón seleccionado: " + currentTargetButton.name);

            currentTargetButton.onClick.Invoke();
        }
        else
        {
            Debug.Log("No hay ningún botón bajo la mira.");
        }

    }

    private void UpdateButtonFeedback()
    {
        currentTargetButton = GetButtonUnderCrosshair();

        // Si seguimos apuntando al mismo botón, no hacemos nada.
        if (currentTargetButton == previousTargetButton)
            return;

        // Quitar el resaltado del botón anterior.
        if (previousTargetButton != null)
        {
            previousTargetButton.OnPointerExit(
                new PointerEventData(EventSystem.current)
            );
        }

        // Resaltar el nuevo botón.
        if (currentTargetButton != null)
        {
            currentTargetButton.OnPointerEnter(
                new PointerEventData(EventSystem.current)
            );
        }

        previousTargetButton = currentTargetButton;
    }

    public void CantShootAnymore()
    {
        canShoot = false;
    }

    public void CallDelayShootUI()
    {
        StartCoroutine(DelayShootUI());
    }

    public IEnumerator DelayShootUI()
    {
        canShoot = false;

        yield return new WaitForSeconds(delayAfterShootUI);

        canShoot = true;
    }
}
