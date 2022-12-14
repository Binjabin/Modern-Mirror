using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsButton : MonoBehaviour
{
    [SerializeField] float threshold = 0.1f;
    [SerializeField] float deadZone = 0.025f;

    bool isPressed;
    Vector3 startPos;
    ConfigurableJoint joint;

    public UnityEvent onPressed, onReleased;

    [SerializeField] AudioClip buttonPressAudio;
    AudioSource buttonPressAudioSource;
    [SerializeField] AudioClip buttonReleaseAudio;
    AudioSource buttonReleaseAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.localPosition;
        joint = GetComponent<ConfigurableJoint>();

        if (buttonPressAudio != null)
        {
            buttonPressAudioSource = gameObject.AddComponent<AudioSource>();
            buttonPressAudioSource.playOnAwake = false;
            buttonPressAudioSource.clip = buttonPressAudio;
        }
        if (buttonReleaseAudio != null)
        {
            buttonReleaseAudioSource = gameObject.AddComponent<AudioSource>();
            buttonReleaseAudioSource.playOnAwake = false;
            buttonReleaseAudioSource.clip = buttonReleaseAudio;
        }
    }

    private void Pressed()
    {
        isPressed = true;
        onPressed.Invoke();
        Audio.AttemptPlayAudio(buttonPressAudioSource);
        
    }

    private void Released()
    {
        isPressed = false;
        onReleased.Invoke();
        Audio.AttemptPlayAudio(buttonReleaseAudioSource);
    }

    void Update()
    {
        if(!isPressed && GetValue() + threshold >= 1)
        {
            Pressed();
        }

        if(isPressed && GetValue() - threshold <= 0)
        {
            Released();
        }
        

    }

    float GetValue()
    {
        float value = Vector3.Distance(startPos, transform.localPosition) / joint.linearLimit.limit;
        if(Mathf.Abs(value) < deadZone)
        {
            value = 0f;
        }
        //Debug.Log(value);
        return Mathf.Clamp(value, -1f, 1f);
    }
}
