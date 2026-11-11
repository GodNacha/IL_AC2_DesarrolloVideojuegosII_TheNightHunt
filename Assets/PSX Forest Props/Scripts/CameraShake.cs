using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    [Header("Shake Settings - Round Start")]
    public float duration = 0.5f;
    public float magnitude = 0.15f;

    [Header("Shake Settings - Shoot")]
    public float durationPistolShake = 0.15f;
    public float magnitudePistolShake = 0.035f;

    private Vector3 initialPosition;
    private Coroutine shakeRoundCoroutine;
    private Coroutine shakePistolCoroutine;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    public void ShakePistol()
    {
        if (shakePistolCoroutine != null)
            StopCoroutine(shakePistolCoroutine);

        shakePistolCoroutine = StartCoroutine(ShakeEffectPistol());
    }

    public void ShakeRound()
    {
        if (shakeRoundCoroutine != null)
            StopCoroutine(shakeRoundCoroutine);

        shakeRoundCoroutine = StartCoroutine(ShakeEffectRound());
    }

    private IEnumerator ShakeEffectRound()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition =
                initialPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = initialPosition;
        shakeRoundCoroutine = null;
    }

    private IEnumerator ShakeEffectPistol()
    {
        float elapsed = 0f;

        while (elapsed < durationPistolShake)
        {
            float x = Random.Range(-1f, 1f) * magnitudePistolShake;
            float y = Random.Range(-1f, 1f) * magnitudePistolShake;

            transform.localPosition =
                initialPosition + new Vector3(x, y, 0f);

            elapsed += Time.deltaTime;

            yield return null;
        }

        transform.localPosition = initialPosition;
        shakePistolCoroutine = null;
    }
}