using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadScenes : MonoBehaviour
{
    public CanvasGroup blackPanel;

    public float fadeDuration = 2f;

    private void Awake()
    {
        Time.timeScale = 1;
    }

    private void Start()
    {       
        blackPanel = GameObject.Find("BlackPanel").GetComponent<CanvasGroup>();
        blackPanel.alpha = 1f;
        StartCoroutine(FadeOutScene());
    }

    public void LoadScene(string levelName)
    {
        StartCoroutine(FadeInScene(levelName));
    }

    public IEnumerator FadeInScene(string level)
    {
        // Bloquear interacciones mientras aparece el negro.
        blackPanel.interactable = false;
        blackPanel.blocksRaycasts = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            blackPanel.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsed / fadeDuration
            );

            yield return null;
        }

        blackPanel.alpha = 1f;

        SceneManager.LoadScene(level);
    }

    IEnumerator FadeInQuitGame()
    {
        // Bloquear interacciones mientras aparece el negro.
        blackPanel.interactable = false;
        blackPanel.blocksRaycasts = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            blackPanel.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsed / fadeDuration
            );

            yield return null;
        }

        blackPanel.alpha = 1f;

        Application.Quit();
    }

    public IEnumerator FadeOutScene()
    {
        // Mantener bloqueadas las interacciones durante la transición.
        blackPanel.interactable = false;
        blackPanel.blocksRaycasts = true;

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            blackPanel.alpha = Mathf.Lerp(
                1f,
                0f,
                elapsed / fadeDuration
            );

            yield return null;
        }

        blackPanel.alpha = 0f;
        blackPanel.interactable = false;
        blackPanel.blocksRaycasts = false;
    }

    public void QuitGame()
    {
        StartCoroutine(FadeInQuitGame());
    }
}
