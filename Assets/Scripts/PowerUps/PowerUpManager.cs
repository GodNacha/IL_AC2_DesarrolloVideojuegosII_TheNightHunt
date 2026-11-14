using UnityEngine;
using TMPro;
using System.Collections;

public class PowerUpManager : MonoBehaviour
{
    public static PowerUpManager Instance;

    [Header("References")]
    public Pistol pistol;

    [Header("Chargers Extra")]
    public int ammoAmount = 1;

    [Header("Extra Damage")]
    public int extraDamageAmount = 2;

    private Coroutine doublePointsCoroutine;
    private Coroutine extraDamageCoroutine;

    [Header("UI")]
    public GameObject textDoblePointFeedback;
    public GameObject textExtraDamageFeedback;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        textDoblePointFeedback.SetActive(false);
        textExtraDamageFeedback.SetActive(false);
    }

    // =========================
    // DOUBLE POINTS
    // =========================

    public void ActivateDoublePoints(float duration)
    {
        if (doublePointsCoroutine != null)
            StopCoroutine(doublePointsCoroutine);

        doublePointsCoroutine = StartCoroutine(
            DoublePointsRoutine(duration)
        );
    }

    private IEnumerator DoublePointsRoutine(float duration)
    {
        ScoreManager.Instance.SetDoublePoints(true);

        textDoblePointFeedback.SetActive(true); //Mostrar feedback

        Debug.Log("¡DOBLE PUNTUACIÓN ACTIVADA!");

        yield return new WaitForSeconds(duration);

        ScoreManager.Instance.SetDoublePoints(false);

        textDoblePointFeedback.SetActive(false); //Descativar feedback

        Debug.Log("Doble puntuación finalizada.");

        doublePointsCoroutine = null;
    }

    // =========================
    // EXTRA DAMAGE
    // =========================

    public void ActivateExtraDamage(float duration)
    {
        if (extraDamageCoroutine != null)
            StopCoroutine(extraDamageCoroutine);

        extraDamageCoroutine = StartCoroutine(
            ExtraDamageRoutine(duration)
        );
    }

    private IEnumerator ExtraDamageRoutine(float duration)
    {
        pistol.SetExtraDamage(extraDamageAmount);

        textExtraDamageFeedback.SetActive(true);

        Debug.Log("¡DAÑO EXTRA ACTIVADO!");

        yield return new WaitForSeconds(duration);

        pistol.RemoveExtraDamage();

        textExtraDamageFeedback.SetActive(false);

        Debug.Log("Daño extra finalizado.");

        extraDamageCoroutine = null;
    }

    public void AddAmmo()
    {
        Pistol pistol = FindFirstObjectByType<Pistol>();

        if (pistol != null)
        {
            pistol.AddAmmo(ammoAmount);
        }

    }

}