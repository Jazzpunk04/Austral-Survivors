using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public GameObject[] disableOnPauseUI;
    public bool IsPaused { get; private set; }
    private float _previousTimeScale = 1f;

    void Awake()
    {
        IsPaused = false;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        Time.timeScale = _previousTimeScale;

    }

    void Update()
    {
        Debug.Log("IsPaused: " + IsPaused);

    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    private void PauseGame()
    {
        if (IsPaused) return;

        _previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        IsPaused = true;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            foreach (GameObject obj in disableOnPauseUI)
            {
                obj.SetActive(false);
            }
        }
    }

    private void ResumeGame()
    {
        if (!IsPaused) return;

        Time.timeScale = _previousTimeScale;
        IsPaused = false;
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
            foreach (GameObject obj in disableOnPauseUI)
            {
                obj.SetActive(true);
            }
        }
    }
}
