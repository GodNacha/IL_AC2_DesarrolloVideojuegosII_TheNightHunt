using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class RaycastPlayer : MonoBehaviour
{
    [Header("Puntero")]
    [SerializeField] private Image crosshair;
    [SerializeField] private float normalSize = 20f;
    [SerializeField] private float hoverSize = 32f;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = Color.yellow;

    [Header("Raycast")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float pickupRange = 8f;
    [SerializeField] private LayerMask pickUpLayer;

    private RectTransform crosshairRect;

    private void Start()
    {
        crosshairRect = crosshair.rectTransform;
        ResetHover();
    }

    private void Update()
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
            FeedbackHover();
        }
        else
        {
            ResetHover();
        }
    }

    public void FeedbackHover()
    {
        crosshairRect.sizeDelta = new Vector2(hoverSize, hoverSize);
        crosshair.color = hoverColor;
    }

    private void ResetHover()
    {
        crosshairRect.sizeDelta = new Vector2(normalSize, normalSize);
        crosshair.color = normalColor;
    }
}
