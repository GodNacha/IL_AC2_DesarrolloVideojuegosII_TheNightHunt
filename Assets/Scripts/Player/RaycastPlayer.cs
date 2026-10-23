using UnityEngine;
using UnityEngine.Rendering;

public class RaycastPlayer : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform holdPoint;

    [Header("Configuración")]
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private LayerMask pickUpLayer = ~0;

    private TrashObject heldTrash;
    private Rigidbody heldRigidbody;

    private bool originalUseGravity;
    private bool originalIsKinematic;

    private void Start()
    {
        playerCamera = GetComponent<Camera>();
       
        holdPoint = GameObject.Find("HoldPoint").GetComponent<Transform>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (heldTrash == null)
            {
                TryPickup();
            }
            else
            {
                Drop();
            }
        }
    }

    private void LateUpdate()
    {
        if (heldTrash == null) return;

        MoveHeldObject();
    }

    private void TryPickup()
    {
        Ray ray = playerCamera.ViewportPointToRay(
            new Vector3(0.5f, 0.5f, 0f)
        );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            pickupRange,
            pickUpLayer,
            QueryTriggerInteraction.Ignore))
        {
            TrashObject trash =
                hit.collider.GetComponentInParent<TrashObject>();

            if (trash == null) return;

            Pickup(trash);
        }
    }

    private void Pickup(TrashObject trash)
    {
        heldTrash = trash;
        heldRigidbody = trash.GetComponent<Rigidbody>();

        if (heldRigidbody != null)
        {
            originalUseGravity = heldRigidbody.useGravity;
            originalIsKinematic = heldRigidbody.isKinematic;

            heldRigidbody.linearVelocity = Vector3.zero;
            heldRigidbody.angularVelocity = Vector3.zero;

            heldRigidbody.useGravity = false;
            heldRigidbody.isKinematic = true;
        }
    }

    private void MoveHeldObject()
    {
        Transform obj = heldTrash.transform;

        obj.position = Vector3.Lerp(
            obj.position,
            holdPoint.position,
            moveSpeed * Time.deltaTime
        );

        obj.rotation = Quaternion.Slerp(
            obj.rotation,
            holdPoint.rotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void Drop()
    {
        if (heldTrash == null) return;

        if (heldRigidbody != null)
        {
            heldRigidbody.useGravity = originalUseGravity;
            heldRigidbody.isKinematic = originalIsKinematic;
        }

        heldTrash = null;
        heldRigidbody = null;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null) return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward * pickupRange
        );
    }
}
