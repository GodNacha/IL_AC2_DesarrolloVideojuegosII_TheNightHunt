using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    [Header("Panels")]
    public CanvasGroup defeatPanel;
    public GameObject winCanvas;

    [Header("Gamer Over Settings")]
    public GameObject defeatPanelText;

    public bool gameOver = false;

    public float fadeGameOver = 1.5f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gameOverSFX;

    private RoundManager roundManager;
    private ScoreManager score;
    private PlayerController player;
    private Pistol pistol;

    private void Start()
    {
        defeatPanel.alpha = 0;
        defeatPanelText.SetActive(false);

        roundManager = FindAnyObjectByType<RoundManager>();
        score = FindAnyObjectByType<ScoreManager>();
        winCanvas.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        player = FindAnyObjectByType<PlayerController>();
        pistol = FindAnyObjectByType<Pistol>();
    }

    public void GameOver()
    {
        gameOver = true;

        player.canLook = false;

        audioSource.PlayOneShot(gameOverSFX); //Sonido GameOver

        StartCoroutine(FadeGameOver());
        roundManager.StopAllCoroutinesOnRoad();
        score.TextFinalScore();
    }

    public void Win()
    {
        gameOver = true;
        pistol.ChangePistolUse();
        StartCoroutine(FadeWin());
    }

    IEnumerator FadeGameOver()
    {
        float elapsed = 0f;

        while (elapsed < fadeGameOver)
        {
            elapsed += Time.unscaledDeltaTime;

            defeatPanel.alpha = Mathf.Lerp(
                0f,
                1f,
                elapsed / fadeGameOver
            );

            yield return null;
        }

        defeatPanel.alpha = 1f;

        yield return new WaitForSeconds(1f);

        defeatPanelText.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    IEnumerator FadeWin()
    {
        yield return new WaitForSeconds(2f);
        score.TextFinalScore();
        winCanvas.SetActive(true);

    }
}
