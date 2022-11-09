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
    [SerializeField] bool isLeft;

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
            
            foreach(Paintable paintable in GetComponentsInChildren<Paintable>())
            {
                Destroy(paintable);
            }
            Destroy(spawnedCan.GetComponent<ShoeRotate>());
            if (isLeft)
            {
                spawnedCan.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            }
            else
            {
                spawnedCan.transform.localScale = new Vector3(-0.2f, 0.2f, 0.2f);
            }
        }

    }

}
