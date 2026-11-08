
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;

    [Header("Horizontal Limits (Yaw)")]
    public float minYaw = -90f;
    public float maxYaw = 90f;

    [Header("Vertical Limits (Pitch)")]
    public float minPitch = -90f;
    public float maxPitch = 90f;

    private float yaw;
    private float pitch;

    private float initialYaw;
    private float initialPitch;

    void Start()
    {
        // Guardar la rotacion inicial de la camara.
        Vector3 initialRotation = transform.localEulerAngles;

        initialYaw = initialRotation.y;
        initialPitch = initialRotation.x;

        // Convertir angulos a valores entre -180 y 180.
        if (initialPitch > 180f)
            initialPitch -= 360f;

        yaw = 0f;
        pitch = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();
    }

    void Look()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

        // Movimiento horizontal.
        yaw += mouseX;
        yaw = Mathf.Clamp(yaw, minYaw, maxYaw);

        // Movimiento vertical.
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Aplicar ambas rotaciones.
        transform.localRotation = Quaternion.Euler(
            initialPitch + pitch,
            initialYaw + yaw,
            0f
        );
    }
}
