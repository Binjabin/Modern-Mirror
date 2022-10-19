using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandObjectSensor : MonoBehaviour
{
    GameObject handParent;

    // Start is called before the first frame update
    void Start()
    {
        handParent = gameObject.GetComponentInParent<XRDirectInteractor>().gameObject;

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
            
            interactable.ExitHandProximity(handParent);
        }
    }
}
