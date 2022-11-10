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

    bool basketballIntroDone = false;

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

    [Header("NPC")]
    NPC npc;
    float lastCheck;
    [SerializeField] Transform npcLocation;
    AudioSource voiceLines;
    
    [SerializeField] List<AudioClip> gameIntro = new List<AudioClip>();
    [SerializeField] List<AudioClip> basketballIntro = new List<AudioClip>();
    [SerializeField] List<AudioClip> basketballCountdown = new List<AudioClip>();
    [SerializeField] List<AudioClip> paintingIntro = new List<AudioClip>();
    [SerializeField] List<AudioClip> outro = new List<AudioClip>();

    [SerializeField] List<AudioClip> basketballScore = new List<AudioClip>();
    [SerializeField] List<AudioClip> paintingComment = new List<AudioClip>();

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
        StartCoroutine(PaintingIntroduction());
    }



    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(0.2f);
        ToIntroduction();
    }

    IEnumerator GameIntroduction()
    {
        foreach(AudioClip clip in gameIntro)
        {
            if(introduction.active)
            {
                yield return new WaitForSeconds(3f);
                voiceLines.clip = clip;
                voiceLines.Play();
                while(voiceLines.isPlaying)
                {
                    yield return null;
                    
                }
            }
        }
    }

    IEnumerator Outro()
    {
        foreach(AudioClip clip in outro)
        {
            if(ending.active)
            {
                yield return new WaitForSeconds(3f);
                voiceLines.clip = clip;
                voiceLines.Play();
                while(voiceLines.isPlaying)
                {
                    yield return null;
                    
                }
            }
        }
    }

    IEnumerator BasketballIntroduction()
    {
        foreach(AudioClip clip in basketballIntro)
        {
            if(basketball.active)
            {
                yield return new WaitForSeconds(1.5f);
                voiceLines.clip = clip;
                voiceLines.Play();
                while(voiceLines.isPlaying)
                {
                    yield return null;
                }
            }
        }
        foreach(AudioClip clip in basketballCountdown)
        {
            if(basketball.active)
            {
                yield return new WaitForSeconds(1f);
                voiceLines.clip = clip;
                voiceLines.Play();
            }
        }
        FindObjectOfType<QueuedObjectSpawner>().StartSpawning();
        basketballIntroDone = true;
        Audio.AttemptPlayAudio(basketballMusicAudioSource, 0.2f, 1f);
    }

    IEnumerator PaintingIntroduction()
    {
        Audio.AttemptPlayAudio(paintingMusicAudioSource, 0.2f, 1f);
        foreach(AudioClip clip in paintingIntro)
        {
            if(painting.active)
            {
                yield return new WaitForSeconds(3f);
                voiceLines.clip = clip;
                voiceLines.Play();
                while(voiceLines.isPlaying)
                {
                    yield return null;
                    
                }
            }
        }
        
        Audio.AttemptStopAudio(basketballMusicAudioSource);
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

        StartCoroutine(BasketballIntroduction());
    }

    public void Start()
    {
        inBasketball = false;
        painting.SetActive(false);
        basketball.SetActive(false);
        introduction.SetActive(false);
        ending.SetActive(false);
        StartCoroutine(WaitToStart());
        voiceLines = npcLocation.gameObject.AddComponent<AudioSource>();
        voiceLines.playOnAwake = false;
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
        StartCoroutine(GameIntroduction());
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
        StartCoroutine(Outro());
        //spawn shoes



    }

    private void Update()
    {
        if (inBasketball && basketballIntroDone)
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

        if(npc == null)
        {
            lastCheck += Time.deltaTime;
            if(lastCheck > 1f)
            {
                npc = FindObjectOfType<NPC>();
                lastCheck = 0f;
            }
        }
        else
        {
            npcLocation.position = npc.transform.position;
        }

    }

    
}
