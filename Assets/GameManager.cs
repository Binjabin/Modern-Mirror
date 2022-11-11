using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static List<Color> scoredColors = new List<Color>();
    public GameObject painting;
    public GameObject basketball;
    public GameObject introduction;
    public GameObject ending;

    public TMP_Text timerText;
    public float basketballTime;
    float basketballTimeRemaining;
    bool inBasketball;

    [SerializeField] GameObject shoe;

    [Header("Sound")]
    [SerializeField] AudioClip timerBeepAudio;
    AudioSource timerBeepAudioSource;
    [SerializeField] AudioClip finalTimerBeepAudio;
    AudioSource finalTimerBeepAudioSource;
    [SerializeField] AudioClip basketballMusicAudio;
    AudioSource basketballMusicAudioSource;
    [SerializeField] AudioClip paintingMusicAudio;
    AudioSource paintingMusicAudioSource;

    public RenderTexture picture;

    public void SaveTexture()
    {
        byte[] bytes = toTexture2D(picture).EncodeToPNG();
        //System.IO.File.WriteAllBytes("C:/Users/bsmith2021/Exports/ShoePicture ("+ System.DateTime.Now + ").png", bytes);
        System.IO.File.WriteAllBytes("C:/Users/bsmith2021/Exports/ShoePicture.png", bytes);
    }
    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(4096, 4096, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        Destroy(tex);//prevents memory leak
        return tex;
    }
    public void ToPainting()
    {
        introduction.SetActive(false);
        ending.SetActive(false);
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

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(1f);
        ToIntroduction();
    }

    public void ToBasketball()
    {
        inBasketball = true;
        introduction.SetActive(false);
        painting.SetActive(false);
        basketball.SetActive(true);
        ending.SetActive(false);

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

    public void Start()
    {
        inBasketball = false;
        painting.SetActive(false);
        basketball.SetActive(false);
        introduction.SetActive(false);
        ending.SetActive(false);
        StartCoroutine(WaitToStart());
        
    }

    public void ToIntroduction()
    {
        inBasketball = false;
        painting.SetActive(false);
        basketball.SetActive(false);
        introduction.SetActive(true);
        ending.SetActive(false);
        var singleSpawners = FindObjectsOfType<SingleObjectSpawner>(false);
        foreach (SingleObjectSpawner spawner in singleSpawners)
        {
            spawner.RespawnObject();
        }
    }

    public void ToEnding()
    {
        inBasketball = false;
        painting.SetActive(false);
        basketball.SetActive(false);
        introduction.SetActive(false);
        ending.SetActive(true);
        var singleSpawners = FindObjectsOfType<SingleObjectSpawner>(false);
        foreach (SingleObjectSpawner spawner in singleSpawners)
        {
            if(spawner.isShoe)
            {
                spawner.originalShoe = shoe;
            }
            spawner.RespawnObject();
        }
        //spawn shoes
        StartCoroutine(TakePhoto());


    }

    IEnumerator TakePhoto()
    {
        yield return new WaitForSeconds(1f);
        SaveTexture();
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
