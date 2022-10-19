using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSensor : MonoBehaviour
{
    public List<InteractableObjectExtentions> objectsInZone;
    int objCount;
    [SerializeField] bool sendNotifications = false;
    public bool isHoop = false;
    // Start is called before the first frame update
    void Start()
    {
        objCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        objCount++;
        InteractableObjectExtentions newObject = other.gameObject.GetComponent<InteractableObjectExtentions>();
        if(newObject != null)
        {
            objectsInZone.Add(newObject);
            if(sendNotifications)
            {
                newObject.EnteredSensor(this);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        objCount--;
        InteractableObjectExtentions newObject = other.gameObject.GetComponent<InteractableObjectExtentions>();
        if(newObject != null)
        {
            objectsInZone.Remove(newObject);
            if(sendNotifications)
            {
                newObject.ExitedSensor(this);
            }
        }
    }
}
