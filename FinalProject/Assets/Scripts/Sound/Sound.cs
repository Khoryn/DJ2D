using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{
    [Header("Settings")]
    private AudioSource source;
    public float volume;

    [Header("Main Menu")]
    public AudioClip mainMenu;

    [Header("Death")]
    public AudioClip comePlay;

    [Header("Game Start")]
    [SerializeField] private AudioClip gameStart;

    bool hasPlayed = false;

    private void Start()
    {
        source = GetComponent<AudioSource>();
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
}
