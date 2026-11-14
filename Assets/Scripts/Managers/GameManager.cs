using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public CanvasGroup defeatPanel;
    public GameObject winPanel;

    public GameObject defeatPanelText;

    public bool gameOver = false;

    public float fadeGameOver = 1.5f;

    private RoundManager roundManager;
    private ScoreManager score;

    private void Start()
    {
        defeatPanel.alpha = 0;
        defeatPanelText.SetActive(false);

        roundManager = FindAnyObjectByType<RoundManager>();
        score = FindAnyObjectByType<ScoreManager>();
    }

    public void GameOver()
    {
        gameOver = true;
        StartCoroutine(FadeGameOver());
        roundManager.StopAllCoroutinesOnRoad();
        score.TextFinalScore();
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
}
