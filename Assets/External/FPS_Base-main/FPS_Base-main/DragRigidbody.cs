using UnityEngine;
using UnityEngine.UI;

// Drag a rigidbody with the mouse using a spring joint like Dani did in KARLSON. By Boxply
// https://youtube.com/c/Boxply

[RequireComponent(typeof(Rigidbody))]
public class DragRigidbody : MonoBehaviour
{
    public float force = 600;
	public float damping = 6;
	public float distance = 15;

	public LineRenderer lr;
	public Transform lineRenderLocation;

	// public LayerMask pickLayer;

    Transform jointTrans;
	float dragDepth;

    [Header("Pointer Feedback")]
    [SerializeField] private UnityEngine.UI.Image crosshair;
    [SerializeField] private LayerMask pickUpLayer;

    [SerializeField] private float normalSize = 6f;
    [SerializeField] private float hoverSize = 10f;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    private RectTransform crosshairRect;

	public bool drag;

    private void Start()
    {
		drag = false;
        crosshairRect = crosshair.rectTransform;
        ResetHover();
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        bool isHovering = Physics.Raycast(
            ray,
            out RaycastHit hit,
            distance,
            pickUpLayer,
            QueryTriggerInteraction.Ignore
        );

		if (drag)
		{
            ResetHover();
			return;
        }
		else
		{
            if (isHovering)
            {
                FeedbackHover();
            }
            else
            {
                ResetHover();
            }
        }

		
    }

    private void FeedbackHover()
    {
        crosshairRect.sizeDelta = new Vector2(hoverSize, hoverSize);
        crosshair.color = hoverColor;
    }

    private void ResetHover()
    {
        crosshairRect.sizeDelta = new Vector2(normalSize, normalSize);
        crosshair.color = normalColor;
    }

    void OnMouseDown ()
	{
		HandleInputBegin (Input.mousePosition);
	}
	
	void OnMouseUp ()
	{
		HandleInputEnd (Input.mousePosition);
	}
	
	void OnMouseDrag ()
	{
		HandleInput (Input.mousePosition);
	}
	
	public void HandleInputBegin (Vector3 screenPosition)
	{
		var ray = Camera.main.ScreenPointToRay (screenPosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, distance)) {
			if (hit.transform.gameObject.layer == LayerMask.NameToLayer ("Interactive")) {
				dragDepth = CameraPlane.CameraToPointDepth (Camera.main, hit.point);
				jointTrans = AttachJoint (hit.rigidbody, hit.point);
			}
		}

		lr.positionCount = 2;
	}
	
	public void HandleInput (Vector3 screenPosition)
	{
		if (jointTrans == null)
			return;
		var worldPos = Camera.main.ScreenToWorldPoint (screenPosition);
		jointTrans.position = CameraPlane.ScreenToWorldPlanePoint (Camera.main, dragDepth, screenPosition);
        drag = true;
        DrawRope();
	}
	
	public void HandleInputEnd (Vector3 screenPosition)
	{
		DestroyRope();
        Destroy (jointTrans.gameObject);
		drag = false;

    }
	
	Transform AttachJoint (Rigidbody rb, Vector3 attachmentPosition)
	{
		GameObject go = new GameObject ("Attachment Point");
		go.hideFlags = HideFlags.HideInHierarchy; 
		go.transform.position = attachmentPosition;
		
		var newRb = go.AddComponent<Rigidbody> ();
		newRb.isKinematic = true;
		
		var joint = go.AddComponent<ConfigurableJoint> ();
		joint.connectedBody = rb;
		joint.configuredInWorldSpace = true;
		joint.xDrive = NewJointDrive (force, damping);
		joint.yDrive = NewJointDrive (force, damping);
		joint.zDrive = NewJointDrive (force, damping);
		joint.slerpDrive = NewJointDrive (force, damping);
		joint.rotationDriveMode = RotationDriveMode.Slerp;
		
		return go.transform;
	}

    private JointDrive NewJointDrive(float force, float damping)
    {
        JointDrive drive = new JointDrive();
        drive.positionSpring = force;
        drive.positionDamper = damping;
        drive.maximumForce = Mathf.Infinity;
        return drive;
    }

    private void DrawRope()
	{
		if(jointTrans == null)
		{
			return;
		}

		lr.SetPosition(0, lineRenderLocation.position);
		lr.SetPosition(1, this.transform.position);
    }

	private void DestroyRope()
	{
		lr.positionCount = 0;
	}
}
