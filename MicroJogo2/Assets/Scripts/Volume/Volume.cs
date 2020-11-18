using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Volume : MonoBehaviour
{
    public Slider mainMenuSlider;
    public Slider pauseMenuSlider;
    public AudioMixer mixer;
    public string parameterName;

    private void Awake()
    {
        // Main Menu Slider
        float savedVol = PlayerPrefs.GetFloat(parameterName, mainMenuSlider.maxValue);
        SetVolume(savedVol);
        mainMenuSlider.value = savedVol;
        mainMenuSlider.onValueChanged.AddListener((float _) => SetVolume(_));


        // Pause Menu Slider
        savedVol = PlayerPrefs.GetFloat(parameterName, pauseMenuSlider.maxValue);
        SetVolume(savedVol);
        pauseMenuSlider.value = savedVol;
        pauseMenuSlider.onValueChanged.AddListener((float _) => SetVolume(_));
    }

    private void SetVolume(float _value)
    {
        mixer.SetFloat(parameterName, ConvertToDecibel(_value / mainMenuSlider.maxValue)); //Dividing by max allows arbitrary positive slider maxValue
        PlayerPrefs.SetFloat(parameterName, _value);

        mixer.SetFloat(parameterName, ConvertToDecibel(_value / pauseMenuSlider.maxValue)); //Dividing by max allows arbitrary positive slider maxValue
        PlayerPrefs.SetFloat(parameterName, _value);
    }

    public float ConvertToDecibel(float _value)
    {
        return Mathf.Log10(Mathf.Max(_value, 0.0001f)) * 20f;
    }
}
