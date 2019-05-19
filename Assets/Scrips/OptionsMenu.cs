using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    Resolution[] resolutions;
    public Dropdown resolutionsDD;
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Toggle fullscreenToggle;

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionsDD.ClearOptions();

        int currentResIndex = 0;

        List<string> options = new List<string>();
        for (int i=0; i<resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentResIndex = i;
        }
        resolutionsDD.AddOptions(options);
        resolutionsDD.value = currentResIndex;
        resolutionsDD.RefreshShownValue();

        masterVolumeSlider.value = PlayerPrefs.GetFloat("masterVolume");
        musicVolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("sfxVolume");
        
        if (PlayerPrefs.GetInt("Fullscreen") == 0)
        {
            fullscreenToggle.isOn = false;
        }
        else
        {
            fullscreenToggle.isOn = true;
        }
    }

    public void SetMasterVolume(float masterVolume)
    {
        audioMixer.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
    }

    public void SetMusicVolume(float musicVolume)
    {
        audioMixer.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
    }

    public void SetSFXvolume(float sfxVolume)
    {
        audioMixer.SetFloat("sfxVolume", sfxVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }

    public void SetFullscreen (bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
        if (isFullScreen)
        {
            PlayerPrefs.SetInt("Fullscreen", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Fullscreen", 0);
        }
    }

    public void SetResolutiton (int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        string res = resolution.width + "x" + resolution.height;
        PlayerPrefs.SetString("Resolution", res);
    }

}
