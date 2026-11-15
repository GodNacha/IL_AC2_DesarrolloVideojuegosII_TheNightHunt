
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [Header("Audio Music")]
    public AudioSource audioMusic;
    public AudioClip tutorialMusic;
    public AudioClip gameMusic;

    [Header("Audio Settings")]
    public float fadeMusic = 2f;

    [Header("Audio List")]
    public List<AudioSource> allAudios = new List<AudioSource>();

    // Guarda el volumen original de cada AudioSource
    private Dictionary<AudioSource, float> originalVolumes = new Dictionary<AudioSource, float>();

    private bool isGameMusic = false;

    private void Awake()
    {
        // Busca todos los AudioSource de la escena
        allAudios.AddRange(
            FindObjectsByType<AudioSource>(FindObjectsSortMode.None)
        );

        // Guarda sus volúmenes originales
        foreach (AudioSource audio in allAudios)
        {
            originalVolumes[audio] = audio.volume;
        }
    }

    private void Start()
    {
        StartCoroutine(FadeOutAllAudios());
    }

    public void ChangeMusic()
    {
        if (!isGameMusic)
        {
            StartCoroutine(FadesMusic());
            isGameMusic = true;
        }
    }

    public void CallFadeOutMusic()
    {
        StartCoroutine(FadeOutMusic());
    }

    public void CallFadeInMusic()
    {
        StartCoroutine(FadeInMusic());
    }


    // Todos los audios pasan de volumen 0 a su volumen original
    private IEnumerator FadeOutAllAudios()
    {
        foreach (AudioSource audio in allAudios)
        {
            audio.volume = 0f;
            audio.Play();
        }

        float timer = 0f;

        while (timer < fadeMusic)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeMusic);

            foreach (AudioSource audio in allAudios)
            {
                audio.volume = Mathf.Lerp(0f, originalVolumes[audio], progress);
            }

            yield return null;
        }

        foreach (AudioSource audio in allAudios)
        {
            audio.volume = originalVolumes[audio];
        }
    }

    // Todos los audios pasan de su volumen original a 0
    private IEnumerator FadeInAllAudios()
    {
        float timer = 0f;

        while (timer < fadeMusic)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeMusic);

            foreach (AudioSource audio in allAudios)
            {
                audio.volume = Mathf.Lerp(originalVolumes[audio], 0f, progress);
            }

            yield return null;
        }

        foreach (AudioSource audio in allAudios)
        {
            audio.volume = 0f;
            audio.Stop();
        }
    }

    // La música pasa de volumen 0 a su volumen original
    private IEnumerator FadeOutMusic()
    {
        float targetVolume = originalVolumes[audioMusic];

        audioMusic.volume = 0f;
        audioMusic.Play();

        float timer = 0f;

        while (timer < fadeMusic)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeMusic);

            audioMusic.volume = Mathf.Lerp(0f, targetVolume, progress);

            yield return null;
        }

        audioMusic.volume = targetVolume;
    }

    // La música pasa de su volumen original a 0
    private IEnumerator FadeInMusic()
    {
        float startVolume = audioMusic.volume;
        float timer = 0f;

        while (timer < fadeMusic)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / fadeMusic);

            audioMusic.volume = Mathf.Lerp(startVolume, 0f, progress);

            yield return null;
        }

        audioMusic.volume = 0f;
        audioMusic.Stop();
    }

    // Transición entre música del tutorial y música principal
    private IEnumerator FadesMusic()
    {
        yield return StartCoroutine(FadeInMusic());

        audioMusic.clip = gameMusic;

    }
}
