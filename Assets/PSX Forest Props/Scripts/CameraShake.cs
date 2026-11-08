using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float duration = 0.5f;
    public float magnitude = 0.15f;

    private Vector3 initialPosition;
    private Coroutine shakeCoroutine;

    private void Awake()
    {
        initialPosition = transform.localPosition;
    }

    public void Shake()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeEffect());
    }

    private IEnumerator ShakeEffect()
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
        shakeCoroutine = null;
    }
}