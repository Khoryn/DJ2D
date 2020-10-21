using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class Volume : MonoBehaviour
{
    public Slider slider;
    public AudioMixer mixer;
    public string parameterName;

    private void Awake()
    {
        float savedVol = PlayerPrefs.GetFloat(parameterName, slider.maxValue);
        SetVolume(savedVol);
        slider.value = savedVol;
        slider.onValueChanged.AddListener((float _) => SetVolume(_));
    }

    private void SetVolume(float _value)
    {
        mixer.SetFloat(parameterName, ConvertToDecibel(_value / slider.maxValue)); //Dividing by max allows arbitrary positive slider maxValue
        PlayerPrefs.SetFloat(parameterName, _value);
    }

    public float ConvertToDecibel(float _value)
    {
        return Mathf.Log10(Mathf.Max(_value, 0.0001f)) * 20f;
    }
}
