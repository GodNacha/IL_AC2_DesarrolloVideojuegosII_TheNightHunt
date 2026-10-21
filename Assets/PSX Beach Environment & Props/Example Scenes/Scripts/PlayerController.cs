using UnityEngine;

namespace PSX_Beach_Demo
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float jumpHeight = 2f;
        public float gravity = -20f;

        [Header("Ground Check")]
        public Transform groundCheck;
        public float groundCheckRadius = 0.25f;
        public LayerMask groundMask;

        [Header("Camera")]
        public Transform cameraTransform;
        public float mouseSensitivity = 2f;
        public float minLookAngle = -80f;
        public float maxLookAngle = 80f;

        private UnityEngine.CharacterController controller;

        private Vector3 velocity;
        private float cameraPitch = 0f;
        private bool isGrounded;

        void Start()
        {
            controller = GetComponent<UnityEngine.CharacterController>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            CheckGround();
            Move();
            Look();
        }

        // --------------------------------------------------
        // GROUND CHECK
        // --------------------------------------------------

        void CheckGround()
        {
            Collider[] groundHits = Physics.OverlapSphere(
                groundCheck.position,
                groundCheckRadius,
                groundMask,
                QueryTriggerInteraction.Ignore
            );

            isGrounded = groundHits.Length > 0;
        }

        // --------------------------------------------------
        // MOVEMENT
        // --------------------------------------------------

        void Move()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 forward = cameraTransform.forward;
            Vector3 right = cameraTransform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = forward * vertical + right * horizontal;

            if (move.magnitude > 1f)
                move.Normalize();

            controller.Move(move * moveSpeed * Time.deltaTime);



            if (isGrounded)
            {
                if (velocity.y < 0f)
                    velocity.y = -2f;

                if (Input.GetButtonDown("Jump"))
                {
                    velocity.y = Mathf.Sqrt(
                        jumpHeight * -2f * gravity
                    );
                }
            }


            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);
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

        void OnDrawGizmosSelected()
        {
            if (groundCheck == null)
                return;

            Gizmos.color = Color.green;

            Gizmos.DrawWireSphere(
                groundCheck.position,
                groundCheckRadius
            );
        }
    }
}

