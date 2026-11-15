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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scoreSfx;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateScoreUI();
    }

    public void AddPoints(int amount)
    {
        int finalPoints = amount * scoreMultiplier;

        currentScore += finalPoints;

        audioSource.PlayOneShot(scoreSfx);

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
        {
            scoreText.text = currentScore.ToString();
        }
            
    }

    public void TextFinalScore()
    {
        scoreTextDefeat.text = "Total Score: " + currentScore.ToString();
        scoreTextWin.text = "Total Score: " + currentScore.ToString();
    }
}