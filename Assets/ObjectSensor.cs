using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSensor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    void OnTriggerExit(Collider other)
    {
        Debug.Log("no longer touching " + other);
        Basketball basketball = other.gameObject.GetComponent<Basketball>();
        if (basketball != null)
        {
            Debug.Log(gameObject + "is no Longer near" + other.gameObject);
            basketball.HandExit();
        }
    }
}
