using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Basketball : MonoBehaviour
{
    [SerializeField] LayerMask canTouchHandsLayer;
    [SerializeField] LayerMask cannotTouchHandsLayer;
    [SerializeField] float canTouchHandsCooldown;
    bool isHeld;
    float timeSinceHeld;
    bool handNearby;

    void Start()
    {
        isHeld = false;
        SetLayer(canTouchHandsLayer);
    }

    // Update is called once per frame
    public void Released()
    {
        isHeld = false;
        timeSinceHeld = 0f;
    }

    public void PickedUp()
    {
        timeSinceHeld = 0f;
        isHeld = true;
        handNearby = true;
        SetLayer(cannotTouchHandsLayer);
    }

    void SetLayer(LayerMask layer)
    {
        gameObject.layer = (int)Mathf.Log(layer.value, 2);
        foreach (Transform child in transform)
        {
            child.gameObject.layer = (int)Mathf.Log(layer.value, 2);
        }
    }

    private void Update()
    {
        if (!isHeld)
        {
            if(!handNearby)
            {
                SetLayer(canTouchHandsLayer);
                //timeSinceHeld += Time.deltaTime;
                //if (timeSinceHeld >= canTouchHandsCooldown)
                //{
                //    SetLayer(canTouchHandsLayer);
                //}
            }
        }
    }



    public void HandExit()
    {
        handNearby = false;
        Debug.Log("hand is no longer near me!");
    }
}
