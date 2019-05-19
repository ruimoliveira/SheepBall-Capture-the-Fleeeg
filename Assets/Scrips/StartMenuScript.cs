using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class StartMenuScript : MonoBehaviour
{
    public AudioMixer audioMixer;

    void Start()
    {
        int fullscreen = PlayerPrefs.GetInt("Fullscreen");
        string resolution = PlayerPrefs.GetString("Resolution");
        if (resolution == null)
        {
            resolution = Screen.currentResolution.width + "x" + Screen.currentResolution.height;
            PlayerPrefs.SetString("Resolution", resolution);
        }
        else
        {
            string[] values = resolution.Split('x');
            if (fullscreen == 0)
                Screen.SetResolution(int.Parse(values[0]), int.Parse(values[1]), false);
            else
                Screen.SetResolution(int.Parse(values[0]), int.Parse(values[1]), true);
        }
        
        checkerSetter("masterVolume");
        checkerSetter("musicVolume");
        checkerSetter("sfxVolume");
    }

    void checkerSetter(string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            float value = PlayerPrefs.GetFloat(key);
            audioMixer.SetFloat(key, value);
        }
        else
        {
            PlayerPrefs.SetFloat(key, 0);
        }
    }
    
}
