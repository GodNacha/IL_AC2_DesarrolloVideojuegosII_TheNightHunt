using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{

    [Header("Referencias de UI")]
    public GameObject menuPausaUI;
    public KeyCode pauseKey = KeyCode.P;

    public bool isPaused = false;

    public AudioSource audioSource;
    public AudioClip pauseSfx;

    private GameManager game;

    private void Start()
    {
        menuPausaUI.SetActive(false);
        game = FindAnyObjectByType<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Detecta si se presiona la tecla Escape (o el botón que prefieras)
        if (Input.GetKeyDown(pauseKey) && !game.gameOver)
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                PauseGame();
            }
        }
    }


    public void PauseGame()
    {
        isPaused = true;
        menuPausaUI.SetActive(true);
        audioSource.PlayOneShot(pauseSfx);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Resume()
    {
        isPaused = false;
        menuPausaUI.SetActive(false);
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
