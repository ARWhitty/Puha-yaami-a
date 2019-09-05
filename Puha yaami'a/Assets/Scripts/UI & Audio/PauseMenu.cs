using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject journalMenuUI;

    public Player player;
    public GameManager gm;

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
        settingsMenuUI.SetActive(false);
        journalMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void ReturnToMap()
    {
        //throw new System.NotImplementedException("Not yet implemented");
        Debug.LogWarning("WARNING: this will be hooked up to map, currently here to test journal functionality");
        pauseMenuUI.SetActive(false);
        journalMenuUI.SetActive(true);
    }

    public void LoadSettingsMenu()
    {
        //throw new System.NotImplementedException("Not yet implemented");
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(true);
    }

    public void LoadMainMenu()
    {
        SaveSystem.SaveAll(player, gm);
        Time.timeScale = 1f;
        Debug.LogError("Main Menu scene not created yet, nothing to load");
        //SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        SaveSystem.SaveAll(player, gm);
        Debug.Log("save complete");
        //TODO: add save call/event here
        Application.Quit();
    }
}
