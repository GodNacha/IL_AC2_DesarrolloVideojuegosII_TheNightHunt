using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{

    [Header("Referencias de UI")]
    public GameObject menuPausaUI;
    public KeyCode pauseKey = KeyCode.P;

    public bool isPaused = false;

    private void Start()
    {
        menuPausaUI.SetActive(false);
    }

    private void Update()
    {
        // Detecta si se presiona la tecla Escape (o el botón que prefieras)
        if (Input.GetKeyDown(pauseKey))
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
