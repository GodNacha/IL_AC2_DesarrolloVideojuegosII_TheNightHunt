using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public RoundManager roundManager;
    public int targetsRemaining = 3;

    private void Awake()
    {
        Instance = this;

    }

    public void TargetDestroyed()
    {
        targetsRemaining--;

        Debug.Log("Dianas restantes: " + targetsRemaining);

        if (targetsRemaining <= 0)
        {
            Debug.Log("¡Tutorial completado!");
            roundManager.StartGame();
        }
    }
}