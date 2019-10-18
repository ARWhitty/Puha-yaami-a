using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject albumMenuUI;
    [SerializeField] private GameObject poemsMenuUI;

    public Player player;
    public GameManager gm;
    private GameObject currentUI;

    private void Start()
    {
        currentUI = pauseMenuUI;
    }

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
        currentUI = pauseMenuUI;
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        settingsMenuUI.SetActive(false);
        albumMenuUI.SetActive(false);
        poemsMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void ReturnToMap()
    {
        //throw new System.NotImplementedException("Not yet implemented");
        Debug.LogWarning("WARNING: this will be hooked up to map, currently here to test journal functionality");
        pauseMenuUI.SetActive(false);
        //albumMenuUI.SetActive(true);
    }

    public void LoadSettingsMenu()
    {
        //throw new System.NotImplementedException("Not yet implemented");
        currentUI.SetActive(false);
        settingsMenuUI.SetActive(true);
        currentUI = settingsMenuUI;
    }

    public void ReturnToMainMenu()
    {
     //   SaveSystem.SaveAll(player, gm);
        Time.timeScale = 1f;
        //SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting");
        SaveSystem.SaveAll(player, gm);
        Debug.Log("save complete");
        //TODO: add save call/event here
        Application.Quit();
    }

    public void LoadPauseMenu()
    {
        currentUI.SetActive(false);
        pauseMenuUI.SetActive(true);
        currentUI = pauseMenuUI;
    }

    public void LoadAlbumMenu()
    {
        currentUI.SetActive(false);
        albumMenuUI.SetActive(true);
        currentUI = albumMenuUI;
    }

    public void LoadPoemsMenu()
    {
        print("poems");
        currentUI.SetActive(false);
        poemsMenuUI.SetActive(true);
        currentUI = poemsMenuUI;
    }
}
