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

    [Header("Despawner")]
    [SerializeField] float despawnTime;
    [SerializeField] MeshRenderer renderer;
    float despawnTimer;
    QueuedObjectSpawner spawner;

    void Start()
    {
        despawnTimer = 0f;
        hands = FindObjectsOfType<XRDirectInteractor>();
        handNearby = false;
        isHeld = false;
        SetLayer(canTouchHandsLayer);

        renderer = GetComponent<MeshRenderer>();
        if(renderer == null)
        {
            renderer = GetComponentInChildren<MeshRenderer>();
        }
    }

    // Update is called once per frame
    public void Released()
    {
        isHeld = false;
    }
    public void LinkSpawner(QueuedObjectSpawner linkThis)
    {
        spawner = linkThis;
    }
    public void PickedUp()
    {
        
        isHeld = true;
        handNearby = true;

        if (dynamicCollisions)
        {
            Debug.Log("changed layer");
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
            Debug.Log("a hand left proximity. Checking for: " + releasedByObject + ". Event triggered by " + hand);
            if (!isHeld)
            {
                
                if (releasedByObject == hand)
                {
                    Debug.Log("most recent holding hand left proximity");
                    SetLayer(canTouchHandsLayer);
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
        else
        {
            Debug.Log("object flip called with no hand found");
        }
        
    }

    void DespawnObject()
    {
        if(spawner != null)
        {
            
            Color color = renderer.material.GetColor("_BaseColor");
            spawner.RemoveColor(color);
        }
        Destroy(gameObject);
    }

    private void Update()
    {
        if (!isHeld)
        {
            despawnTimer += Time.deltaTime;
            if(despawnTimer >= despawnTime)
            {
                DespawnObject();
            }
            if (!handNearby)
            {
                
                SetLayer(canTouchHandsLayer);
            }
        }
        else
        {
            despawnTimer = 0f;
        }

    }

    
    GameObject GetHoldingHand()
    {
        GameObject holdingHand = null;
        if (isHeld)
        {
            for (int i = 0; i < hands.Length; i++)
            {
                Debug.Log(hands[i]);
                if(hands[i].interactablesSelected.Count > 0)
                {
                    if (hands[i].interactablesSelected[0].transform.gameObject == gameObject)
                    {
                        Debug.Log(hands[i].interactablesSelected[0].transform.gameObject);
                        holdingHand = hands[i].gameObject;
                    }

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
