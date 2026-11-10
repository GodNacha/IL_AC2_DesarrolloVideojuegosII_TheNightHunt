using TMPro;
using UnityEngine;

public enum PowerUpType
{
    DoublePoints,
    ExtraDamage,
    ExtraCharger
}

public class PowerUp : MonoBehaviour, IShootable
{
    [Header("Power Up")]
    public PowerUpType powerUpType;

    [Header("Duration")]
    public float durationDoblePoints = 8f;
    public float durationExtraDamage = 5f;

    private bool collected;

    public void OnShot()
    {
        if (collected) return;

        collected = true;

        switch (powerUpType)
        {
            case PowerUpType.DoublePoints:

                PowerUpManager.Instance.ActivateDoublePoints(durationDoblePoints);

                break;

            case PowerUpType.ExtraDamage:

                PowerUpManager.Instance.ActivateExtraDamage(durationExtraDamage);

                break;

            case PowerUpType.ExtraCharger:

                PowerUpManager.Instance.AddAmmo();

                break;
        }

        Destroy(gameObject);
    }
}