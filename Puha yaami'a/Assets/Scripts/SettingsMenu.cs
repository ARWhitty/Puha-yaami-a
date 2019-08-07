using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject previousUI;

    [SerializeField] private Dropdown resolutionDropdown;

    Resolution[] possibleResolutions;

    private void Start()
    {
        //get all unity inbuilt options
        possibleResolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        List<string> resNames = new List<string>();

        int currentResIdx = 0;
        for(int i = 0; i < possibleResolutions.Length; i++)
        {
            resNames.Add(possibleResolutions[i].width + " x " + possibleResolutions[i].height);

            if(possibleResolutions[i].width == Screen.currentResolution.width && possibleResolutions[i].height == Screen.currentResolution.height)
            {
                currentResIdx = i;
            }
        }

        //set to default
        resolutionDropdown.AddOptions(resNames);
        resolutionDropdown.value = currentResIdx;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        throw new System.NotImplementedException("No Audio Yet");
    }

    public void SetQuality(int qualityIdx)
    {
        QualitySettings.SetQualityLevel(qualityIdx);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
    }

    public void SetResolution(int resIdx)
    {
        Resolution newRes = possibleResolutions[resIdx];
        Screen.SetResolution(newRes.width, newRes.height, Screen.fullScreen);
    }

    public void ReturnToPreviousMenu()
    {
        settingsMenuUI.SetActive(false);
        previousUI.SetActive(true);
    }
}
