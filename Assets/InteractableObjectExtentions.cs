using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class InteractableObjectExtentions : MonoBehaviour
{
    [Header("Hands")]
    public XRDirectInteractor[] hands;

    [Header("Collision Settings")]
    [SerializeField] bool dynamicCollisions;
    [SerializeField] LayerMask canTouchHandsLayer;
    [SerializeField] LayerMask cannotTouchHandsLayer;
    bool isHeld;
    bool handNearby;
    GameObject releasedByObject;

    [Header("Object Flip")]
    [SerializeField] bool doObjectFlip;
    [SerializeField] GameObject holdingPivotPoint;
    [SerializeField] Vector3 leftHandRotation;
    [SerializeField] Vector3 rightHandRotation;


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
    }

    public void PickedUp()
    {
        isHeld = true;
        handNearby = true;

        if (dynamicCollisions)
        {
            SetLayer(cannotTouchHandsLayer);
            releasedByObject = GetHoldingHand();
        }

        if (doObjectFlip)
        {
            DoObjectFlip();
        }

    }

    public void ExitHandProximity(GameObject hand)
    {
        if(dynamicCollisions)
        {
            if (!isHeld)
            {
                if (releasedByObject == hand)
                {
                    handNearby = false;
                }
            }
        }
    }


    //sets the layer of all child colliders
    void SetLayer(LayerMask layer)
    {
        gameObject.layer = (int)Mathf.Log(layer.value, 2);
        foreach (Transform child in transform)
        {
            child.gameObject.layer = (int)Mathf.Log(layer.value, 2);
        }
    }

    void DoObjectFlip()
    {
        //Flip object depending on hands
        GameObject currentHand = GetHoldingHand();
        if (currentHand != null)
        {
            if (currentHand.tag == "LeftHand")
            {
                holdingPivotPoint.transform.localEulerAngles = leftHandRotation;
            }
            else if (currentHand.tag == "RightHand")
            {
                holdingPivotPoint.transform.localEulerAngles = rightHandRotation;
            }

        }
    }

    

    private void Update()
    {
        if (!isHeld)
        {
            if (!handNearby)
            {
                SetLayer(canTouchHandsLayer);
            }
        }

    }

    
    GameObject GetHoldingHand()
    {
        GameObject holdingHand = null;
        if (isHeld)
        {
            for (int i = 0; i < hands.Length; i++)
            {
                if(hands[i].interactablesSelected[0].transform.gameObject == gameObject.transform.parent.gameObject)
                {
                    holdingHand = hands[i].interactablesSelected[0].transform.gameObject;
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
