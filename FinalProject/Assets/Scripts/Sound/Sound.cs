using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    [Header("Settings")]
    public AudioSource source;
    public AudioMixer mixer;
    public float volume;

    [Header("Main Menu")]
    public AudioClip mainMenu;

    [Header("Death")]
    public AudioClip comePlay;

    [Header("Game Start")]
    [SerializeField] private AudioClip gameStart;

    [HideInInspector]
    public bool hasPlayed = false;

    GuiManager gui;

    private void Start()
    {
        gui = FindObjectOfType<GuiManager>();
        source = GetComponent<AudioSource>();

        // Load sound values
        gui.masterSlider.value = PlayerPrefs.GetFloat("Master", 0.75f);
        gui.effectsSlider.value = PlayerPrefs.GetFloat("Effects", 0.75f);
        gui.musicSlider.value = PlayerPrefs.GetFloat("Music", 0.75f);
        gui.ambientSlider.value = PlayerPrefs.GetFloat("Ambient", 0.75f);

        //gui.masterSliderPause.value = PlayerPrefs.GetFloat("Master", 0.75f);
        //gui.effectsSliderPause.value = PlayerPrefs.GetFloat("Effects", 0.75f);
        //gui.musicSliderPause.value = PlayerPrefs.GetFloat("Music", 0.75f);
        //gui.ambientSliderPause.value = PlayerPrefs.GetFloat("Ambient", 0.75f);
    }

    private void Update()
    {
        
    }

    public void PlayOnGameStart()
    {
        source.PlayOneShot(gameStart, volume);
    }

    public void PlayMainMenuMusic(bool canPlay)
    {
        if (canPlay)
        {
            source.clip = mainMenu;
            source.Play();
            source.loop = true;
        }
        else
        {
            source.clip = null;
            source.loop = false;
        }
    }

    public void ComePlayGameSound(float time)
    {
        if (!hasPlayed)
        {
            source.PlayOneShot(comePlay, volume);
            hasPlayed = true;
        }
        Invoke("StopAudio", time);
    }

    private void StopAudio()
    {
        GetComponent<AudioSource>().Stop();
    }

    public IEnumerator ComePlaySound()
    {
        bool hasPlayed = false;

        if (!hasPlayed)
        {
            source.clip = comePlay;
            source.Play();
            hasPlayed = true;
        }
        yield return new WaitForSeconds(5f);
        source.clip = null;
    }

    public void SetMasterLevel()
    {
        float sliderValue = gui.masterSlider.value;
        mixer.SetFloat("Master", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Master", sliderValue);
    }

    public void SetEffectsLevel()
    {
        float sliderValue = gui.effectsSlider.value;
        mixer.SetFloat("Effects", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Effects", sliderValue);
    }

    public void SetMusicLevel()
    {
        float sliderValue = gui.musicSlider.value;
        mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Music", sliderValue);
    }

    public void SetAmbientLevel()
    {
        float sliderValue = gui.ambientSlider.value;
        mixer.SetFloat("Ambient", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Ambient", sliderValue);
    }

    // Pause Menu

    public void SetMasterLevelPause()
    {
        float sliderValue = gui.masterSliderPause.value;
        mixer.SetFloat("Master", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Master", sliderValue);
    }

    public void SetEffectsLevelPause()
    {
        float sliderValue = gui.effectsSliderPause.value;
        mixer.SetFloat("Effects", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Effects", sliderValue);
    }

    public void SetMusicLevelPause()
    {
        float sliderValue = gui.musicSliderPause.value;
        mixer.SetFloat("Music", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Music", sliderValue);
    }

    public void SetAmbientLevelPause()
    {
        float sliderValue = gui.ambientSliderPause.value;
        mixer.SetFloat("Ambient", Mathf.Log10(sliderValue) * 20);
        PlayerPrefs.SetFloat("Ambient", sliderValue);
    }
}
