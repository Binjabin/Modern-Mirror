using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleObjectSpawner : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    public GameObject spawnPrefab;
    GameObject currentObject;
    public ObjectSensor safeZone;

    public bool isShoe;
    public GameObject originalShoe;

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
        spawnedCan.transform.parent = transform.parent;

        spawnedCan.GetComponent<InteractableObjectExtentions>().LinkSpawner(this);
        

        if(isShoe)
        {
            
            Destroy(spawnedCan.GetComponent<ShoeRotate>());
            foreach (Transform child in originalShoe.transform)
            {
                var copied = Instantiate(child.gameObject);
                copied.transform.parent = spawnedCan.transform;
                copied.transform.localPosition = child.localPosition;
                copied.transform.localEulerAngles = child.localEulerAngles;
                copied.transform.localScale = child.localScale;
                
            }
        }

    }

}
