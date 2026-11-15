using System.Collections;
using TMPro;
using Unity.Collections;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public RoundManager roundManager;
    public int targetsRemaining = 3;

    [Header("Tragets")]
    public GameObject dianas;
    public GameObject powerUpTutorial;

    [Header("TutorialText")]
    public GameObject controlText;
    public GameObject task1Text;
    public GameObject task2Text;
    public GameObject startGameText;

    [Header("Start")]
    public int addAmmoStart = 1;

    private MusicManager music;
    private Pistol pistol;

    private void Awake()
    {
        Instance = this;

        music = FindAnyObjectByType<MusicManager>();
        pistol = FindAnyObjectByType<Pistol>();

        powerUpTutorial.SetActive(false);

        powerUpTutorial.SetActive(false);

        task1Text.SetActive(false);
        task2Text.SetActive(false);
        startGameText.SetActive(false);
        controlText.SetActive(false);
        dianas.SetActive(false);

        StartCoroutine(delayStart());
    }

    public void TargetDestroyed()
    {
        targetsRemaining--;

        Debug.Log("Dianas restantes: " + targetsRemaining);

        if (targetsRemaining == 1)
        {
            powerUpTutorial.SetActive(true);

            task1Text.SetActive(false);
            task2Text.SetActive(true);
            powerUpTutorial.SetActive(true);

        }

        if (targetsRemaining <= 0)
        {
            Debug.Log("¡Tutorial completado!");

            music.ChangeMusic();

            StartCoroutine(delayStartText());
        }
    }

    IEnumerator delayStartText()
    {
        controlText.SetActive(false);
        task2Text.SetActive(false);
        startGameText.SetActive(true);
        yield return new WaitForSeconds(2f);
        startGameText.SetActive(false);
        roundManager.StartGame();

        pistol.AddAmmo(addAmmoStart);
    }

    IEnumerator delayStart()
    {
        controlText.SetActive(true);

        yield return new WaitForSeconds(2f);

        task1Text.SetActive(true);

        yield return new WaitForSeconds(1f);

        dianas.SetActive(true);

    }
}