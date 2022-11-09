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
            if (isLeft)
            {
                spawnedCan.transform.localScale = new Vector3(0.06f, 0.06f, 0.06f);
            }
            else
            {
                spawnedCan.transform.localScale = new Vector3(-0.06f, 0.06f, 0.06f);
            }
            foreach (Transform child in originalShoe.transform)
            {
                var copied = Instantiate(child.gameObject);
                copied.transform.parent = spawnedCan.transform;
                transform.localPosition = new Vector3(0, 0, 0);
                transform.localEulerAngles = new Vector3(-90, 0, 0);
                transform.localScale = child.localScale;
                
            }
        }

    }

}
