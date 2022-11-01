using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandObjectSensor : MonoBehaviour
{
    GameObject handInteractor;

    PhysicsHand physicsHand;

    // Start is called before the first frame update
    void Start()
    {
        physicsHand = gameObject.GetComponentInParent<PhysicsHand>();
        handInteractor = physicsHand.followObject.GetComponent<XRDirectInteractor>().gameObject;

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void OnTriggerExit(Collider other)
    {
        InteractableObjectExtentions interactable = other.gameObject.GetComponent<InteractableObjectExtentions>();
        if (interactable != null)
        {
            
            interactable.ExitHandProximity(handInteractor);
        }
    }
}
