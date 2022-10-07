using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Basketball : MonoBehaviour
{
    [SerializeField] LayerMask canTouchHandsLayer;
    [SerializeField] LayerMask cannotTouchHandsLayer;
    [SerializeField] float canTouchHandsCooldown;
    bool isHeld;
    float timeSinceHeld;
    bool handNearby;
    GameObject releasedByObject;
    XRDirectInteractor[] hands;

    void Start()
    {
        hands = FindObjectsOfType<XRDirectInteractor>();
        handNearby = false;
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
        Debug.Log("picked up" + gameObject);
        releasedByObject = FindHoldingHand();
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



    public void HandExit(GameObject hand)
    {
        if(!isHeld)
        {
            if(releasedByObject)
            {
                handNearby = false;
                Debug.Log("hand is no longer near " + gameObject);
            }
            
        }
        Debug.Log("hand left proximity but is being held");
    }

    GameObject FindHoldingHand()
    {
        GameObject holdingHand = null;
        if(isHeld)
        {
            float minDistance = 1000000f;
            for (int i = 0; i < hands.Length; i++)
            {
                var dist = Vector3.Distance(transform.position, hands[i].transform.position);
                if(dist < minDistance)
                {
                    minDistance = dist;
                    holdingHand = hands[i].gameObject;
                }
            }
            return holdingHand;
        }
        else
        {
            return null;
        }
        
        
    }
}
