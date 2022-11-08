using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoeRotate : MonoBehaviour
{
    [SerializeField] float rotationSpeed;
    bool leftPressed;
    bool rightPressed;

    [SerializeField] AudioClip spinningAudio;
    AudioSource spinningAudioSource;

    private void Start()
    {
        if (spinningAudio != null)
        {
            spinningAudioSource = gameObject.AddComponent<AudioSource>();
            spinningAudioSource.playOnAwake = false;
            spinningAudioSource.clip = spinningAudio;
            spinningAudioSource.loop = true;
        }
    }
    public void LeftButtonPressed()
    {
        leftPressed = true;
    }

    public void LeftButtonReleased()
    {
        leftPressed = false;
    }

    public void RightButtonPressed()
    {
        rightPressed = true;
        Debug.Log("pressed!");
    }

    public void RightButtonReleased()
    {
        rightPressed = false;
        Debug.Log("released!");
    }

    void Update()
    {
        //Debug.Log(leftPressed + " " + rightPressed);
        if(leftPressed)
        {
            if (!rightPressed)
            {
                transform.localEulerAngles += new Vector3(0f, rotationSpeed * Time.deltaTime, 0f);
                Audio.AttemptPlayAudio(spinningAudioSource, 0.8f, 1f);
            }
            else
            {
                Audio.AttemptStopAudio(spinningAudioSource);
            }

        }
        else if(rightPressed)
        {
            transform.localEulerAngles -= new Vector3(0f, rotationSpeed * Time.deltaTime, 0f);
            Audio.AttemptPlayAudio(spinningAudioSource, 0.8f, 1f);
        }
        else
        {
            Audio.AttemptStopAudio(spinningAudioSource);
        }
    }
}
