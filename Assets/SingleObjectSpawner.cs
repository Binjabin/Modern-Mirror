using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleObjectSpawner : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] GameObject spawnPrefab;
    GameObject currentObject;
    public ObjectSensor safeZone;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RespawnObject()
    {
        
        GameObject spawnedCan = Instantiate(spawnPrefab, spawnPoint.position, Quaternion.identity);
        spawnedCan.GetComponent<InteractableObjectExtentions>().LinkSpawner(this);
        
    }

}
