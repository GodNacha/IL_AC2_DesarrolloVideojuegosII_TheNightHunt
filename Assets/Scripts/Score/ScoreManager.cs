using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Score")]
    public int currentScore = 0;
    public int scoreMultiplier = 1;

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text scoreTextDefeat;
    public TMP_Text scoreTextWin;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddPoints(int amount)
    {
        int finalPoints = amount * scoreMultiplier;

        currentScore += finalPoints;

        UpdateScoreUI();

        Debug.Log("Puntos obtenidos: " + finalPoints);
    }

    public void SetDoublePoints(bool active)
    {
        scoreMultiplier = active ? 2 : 1;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = currentScore.ToString();
    }

    public void TextFinalScore()
    {
        scoreTextDefeat.text = "Total Score: " + currentScore.ToString();
        scoreTextWin.text = "Total Score: " + currentScore.ToString();
    }
}