using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static List<Color> scoredColors = new List<Color>();
    public GameObject painting;
    public GameObject basketball;

    public TMP_Text timerText;
    public float basketballTime;
    float basketballTimeRemaining;
    bool inBasketball;

    [Header("Sound")]
    [SerializeField] AudioClip timerBeepAudio;
    AudioSource timerBeepAudioSource;
    [SerializeField] AudioClip finalTimerBeepAudio;
    AudioSource finalTimerBeepAudioSource;
    [SerializeField] AudioClip basketballMusicAudio;
    AudioSource basketballMusicAudioSource;
    [SerializeField] AudioClip paintingMusicAudio;
    AudioSource paintingMusicAudioSource;

    public void ToPainting()
    {
        inBasketball = false;
        painting.SetActive(true);
        basketball.SetActive(false);
        FindObjectOfType<CanSpawner>().SpawnCans();
        var singleSpawners = FindObjectsOfType<SingleObjectSpawner>(false);
        foreach (SingleObjectSpawner spawner in singleSpawners)
        {
            spawner.RespawnObject();
        }
        Audio.AttemptPlayAudio(paintingMusicAudioSource, 0.2f, 1f);
        Audio.AttemptStopAudio(basketballMusicAudioSource);
    }

    public void Start()
    {
        inBasketball = true;
        painting.SetActive(false);
        basketball.SetActive(true);

        basketballTimeRemaining = basketballTime;
        var singleSpawners = FindObjectsOfType<SingleObjectSpawner>(false);
        foreach (SingleObjectSpawner spawner in singleSpawners)
        {
            spawner.RespawnObject();
        }


        if (timerBeepAudio != null)
        {
            timerBeepAudioSource = gameObject.AddComponent<AudioSource>();
            timerBeepAudioSource.playOnAwake = false;
            timerBeepAudioSource.clip = timerBeepAudio;
        }
        if (finalTimerBeepAudio != null)
        {
            finalTimerBeepAudioSource = gameObject.AddComponent<AudioSource>();
            finalTimerBeepAudioSource.playOnAwake = false;
            finalTimerBeepAudioSource.clip = finalTimerBeepAudio;
        }
        if (basketballMusicAudio != null)
        {
            basketballMusicAudioSource = gameObject.AddComponent<AudioSource>();
            basketballMusicAudioSource.playOnAwake = false;
            basketballMusicAudioSource.clip = basketballMusicAudio;
            basketballMusicAudioSource.loop = true;
        }
        if (paintingMusicAudio != null)
        {
            paintingMusicAudioSource = gameObject.AddComponent<AudioSource>();
            paintingMusicAudioSource.playOnAwake = false;
            paintingMusicAudioSource.clip = paintingMusicAudio;
            paintingMusicAudioSource.loop = true;
        }

        Audio.AttemptPlayAudio(basketballMusicAudioSource, 0.2f, 1f);
    }

    private void Update()
    {
        if (inBasketball)
        {
            basketballTimeRemaining -= Time.deltaTime;
            float secs = Mathf.Round(basketballTimeRemaining % 60);
            float mins = Mathf.Floor(basketballTimeRemaining / 60);
            string newText = mins + ":" + secs;
            if (newText != timerText.text)
            {
                //a second has past
                timerText.text = newText;
                if (basketballTimeRemaining < 1f)
                {
                    
                    Audio.AttemptPlayAudio(finalTimerBeepAudioSource, 1f, 1f);
                }
                else if(basketballTimeRemaining < 6f)
                {
                    float pitch = 1 + ((5 - basketballTimeRemaining) / 20f);
                    Audio.AttemptPlayAudio(timerBeepAudioSource, 1f, pitch);
                }
            }

            if (basketballTimeRemaining < 0f)
            {
                ToPainting();

            }
        }

    }

    
}
