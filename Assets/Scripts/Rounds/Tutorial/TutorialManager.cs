using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public RoundManager roundManager;
    public int targetsRemaining = 3;

    private MusicManager music;

    private void Awake()
    {
        Instance = this;

        music = FindAnyObjectByType<MusicManager>();

    }

    public void TargetDestroyed()
    {
        targetsRemaining--;

        Debug.Log("Dianas restantes: " + targetsRemaining);

        if (targetsRemaining <= 0)
        {
            Debug.Log("¡Tutorial completado!");

            music.ChangeMusic();
            roundManager.StartGame();
        }
    }
}