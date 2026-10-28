using UnityEngine;

namespace PSX_Beach_Demo
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Camera")]
        public Transform cameraTransform;
        public float mouseSensitivity = 2f;
        public float minLookAngle = -80f;
        public float maxLookAngle = 80f;

        private UnityEngine.CharacterController controller;

        private float cameraPitch = 0f;

        void Start()
        {
            controller = GetComponent<UnityEngine.CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {       
            Look();
        }

       
        void Look()
        {
            float mouseX =
                Input.GetAxisRaw("Mouse X") * mouseSensitivity;

            float mouseY =
                Input.GetAxisRaw("Mouse Y") * mouseSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            cameraPitch -= mouseY;

            cameraPitch = Mathf.Clamp(
                cameraPitch,
                minLookAngle,
                maxLookAngle
            );

            cameraTransform.localRotation =
                Quaternion.Euler(
                    cameraPitch,
                    0f,
                    0f
                );
        }
      
    }
}

