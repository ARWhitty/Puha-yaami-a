using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Pause"))
        {
            if(gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void ReturnToMap()
    {
        throw new System.NotImplementedException("Not yet implemented");
    }

    public void LoadSettingsMenu()
    {
        //throw new System.NotImplementedException("Not yet implemented");
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void LoadMainMenu()
    {
        //TODO: add save event/call here
        Time.timeScale = 1f;
        Debug.LogError("Main Menu scene not created yet, nothing to load");
        //SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        //TODO: add save call/event here
        Application.Quit();
    }
}
