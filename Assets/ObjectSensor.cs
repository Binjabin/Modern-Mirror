using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectSensor : MonoBehaviour
{
    //use -1 for left, 1 for right
    [SerializeField] int isLeft;
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
        CollisionObject basketball = other.gameObject.GetComponent<CollisionObject>();
        if (basketball != null)
        {
            basketball.HandExit(gameObject);
        }
    }
}
