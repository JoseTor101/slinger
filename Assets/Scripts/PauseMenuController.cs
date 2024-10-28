using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    
    void Start()
    {
        pauseMenuUI.SetActive(false);
    }

    void Update()
    {
        // Detecta la tecla "Esc" para pausar o reanudar
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true); 
        Time.timeScale = 0f; // Detiene el tiempo del juego
        isPaused = true;
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // Restablece el tiempo antes de salir

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false; // Detiene el juego en el editor
        #else
            Application.Quit(); // Cierra la aplicación en una compilación
        #endif
    }

}
