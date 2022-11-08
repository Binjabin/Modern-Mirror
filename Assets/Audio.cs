using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Audio
{
    public static bool AttemptPlayAudio(AudioSource source)
    {
        if (source != null)
        {
            if (!source.isPlaying)
            {
                source.pitch = Random.Range(0.9f, 1.1f);
                source.volume = Random.Range(0.9f, 1.1f);

                source.Play();
                return true;
            }
        }
        return false;
    }

    public static bool AttemptPlayAudio(AudioSource source, float volume)
    {
        if (source != null)
        {
            if (!source.isPlaying)
            {
                source.pitch = Random.Range(0.9f, 1.1f);
                source.volume = volume;

                source.Play();
                return true;
            }
        }
        return false;
    }

    public static bool AttemptPlayAudio(AudioSource source, float volume, float pitch)
    {
        if (source != null)
        {
            if (!source.isPlaying)
            {
                source.pitch = pitch;
                source.volume = volume;

                source.Play();
                return true;
            }
        }
        return false;
    }

    public static void AttemptStopAudio(AudioSource source)
    {
        if (source != null)
        {
            if (source.isPlaying)
            {
                source.Stop();
            }
        }
    }
}
