using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] GameObject eyes;
    Transform player;
    InteractableObjectExtentions objectExtentions;
    

    
    // Start is called before the first frame update
    void Start()
    {
        player = Camera.main.transform;
        objectExtentions = GetComponent<InteractableObjectExtentions>();
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.position - transform.position;
        Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
        if(objectExtentions.despawnTimer == 0f)
        {
            eyes.transform.rotation = rot;
        }
    }


}
