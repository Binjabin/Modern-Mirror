using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuedObjectSpawner : MonoBehaviour
{
    [SerializeField] bool funnyMode;
    [SerializeField] GameObject objectPrefab;
    [SerializeField] float objcount;
    // Start is called before the first frame update
    void Start()
    {
        objcount = 0;
        Instantiate(objectPrefab, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        if(funnyMode)
        {
            Instantiate(objectPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            if(objcount == 0)
            {
                Instantiate(objectPrefab, transform.position, Quaternion.identity);
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        objcount++;
    }
    private void OnTriggerExit(Collider other)
    {
        objcount--;
    }
}
