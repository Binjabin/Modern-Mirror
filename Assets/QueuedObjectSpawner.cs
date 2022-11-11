using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueuedObjectSpawner : MonoBehaviour
{
    [Header("Spawning")]
    [SerializeField] bool funnyMode;
    [SerializeField] GameObject objectPrefab;
    [SerializeField] int objCount;
    [SerializeField] float spawnCooldown = 1f;
    float timeSinceLastSpawn;

    [Header("Colors")]
    [SerializeField] bool randomiseColors = true;
    [SerializeField] List<Color> inSceneColors = new List<Color>();
    public List<Color> scoredColors = new List<Color>();
    [SerializeField] bool addColor;
    [SerializeField] float minimumUniqueColorThreshold;
    float uniqueColorThreshold;

    [Header("Safe Area")]
    public ObjectSensor safeZone;

    HoopController hoopController;

    bool isSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        objCount = 0;
        if(safeZone == null)
        {
            if(gameObject.transform.GetChild(0) != null);
            {
                safeZone = GetComponentInChildren<ObjectSensor>();
            }
            
        }
        hoopController = FindObjectOfType<HoopController>();
    }

    public void StartSpawning()
    {
        isSpawning = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(isSpawning)
        {
            if(funnyMode)
            {
                Instantiate(objectPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                timeSinceLastSpawn += Time.deltaTime;
                if (objCount == 0)
                {
                    
                    if(timeSinceLastSpawn > spawnCooldown)
                    {
                        timeSinceLastSpawn = 0f;
                        GameObject newObject = Instantiate(objectPrefab, transform.position, Quaternion.identity);
                        newObject.transform.parent = gameObject.transform.root;
                        if(randomiseColors)
                        {
                            Color newObjectColor = GetNewColor();
                            newObject.GetComponentInChildren<MeshRenderer>().material.SetColor("_BaseColor", newObjectColor);
                            if(newObject.GetComponentInChildren<ParticleSystemRenderer>() != null)
                            {
                                newObject.GetComponentInChildren<ParticleSystemRenderer>().material.SetColor("_BaseColor", newObjectColor);
                            }
                            newObject.GetComponent<InteractableObjectExtentions>().LinkSpawner(this);
                        }
                    }
                    
                    
                }
            }
        }
    }
    Color GetNewColor()
    {
        uniqueColorThreshold = 1f;
        bool foundColor = false;
        Color newColor = new Vector4(0, 1, 0, 1);
        while (uniqueColorThreshold >= minimumUniqueColorThreshold && foundColor == false)
        {
            newColor = AttemptGetNewColor(uniqueColorThreshold);
            if (newColor.a > 0.0f)
            {
                inSceneColors.Add(newColor);
                foundColor = true;
            }
            else
            {
                uniqueColorThreshold -= 0.03f;
            }
            
        }
        return newColor;
        
    }
    Color AttemptGetNewColor(float threshold)
    {
        bool foundColor = false;
        bool noPossibleColor = false;
        bool attemptedBlack = false;
        bool attemptedWhite = false;
        int attempts = 0;
        while (foundColor == false && noPossibleColor == false)
        {
            Color testColor;
            if (attemptedBlack && attemptedWhite)
            {
                testColor = new Vector4(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
            }
            else if (!attemptedBlack)
            {
                attemptedBlack = true;
                testColor = new Vector4(0, 0, 0, 1);
                
            }
            else
            {
                attemptedWhite = true;
                testColor = new Vector4(1, 1, 1, 1);
            }

            float smallestDist = Mathf.Infinity;
            if (inSceneColors.Count != 0)
            {
                foreach (Color checkColor in scoredColors)
                {
                    float dist = Vector4.Distance(testColor, checkColor);
                    if (dist < smallestDist)
                    {
                        smallestDist = dist;
                    }
                }
                foreach (Color checkColor in inSceneColors)
                {
                    float dist = Vector4.Distance(testColor, checkColor);
                    if (dist < smallestDist)
                    {
                        smallestDist = dist;
                    }
                }
            }
            if (smallestDist > threshold)
            {
                
                foundColor = true;
                return testColor;
                
            }
            attempts++;
            if (attempts > 100)
            {
                noPossibleColor = true;
                
            }
        }
        return new Vector4(0,0,0,0);
    }
    private void OnTriggerEnter(Collider other)
    {
        objCount++;
    }
    private void OnTriggerExit(Collider other)
    {
        objCount--;
    }

    public void ScoreColor(Color color)
    {
        Debug.Log(color);
        List<Color> toRemove = new List<Color>();
        foreach (Color checkColor in inSceneColors)
        {
            float dist = Vector4.Distance(color, checkColor);
            if (dist < 0.01f)
            {
                toRemove.Add(checkColor);
            }
        }
        foreach (Color removeColor in toRemove)
        {
            inSceneColors.Remove(removeColor);
        }
        scoredColors.Add(color);
        GameManager.scoredColors = scoredColors;
        hoopController.HoopScored();
    }

    public void RemoveColor(Color color)
    {
        Debug.Log(color);
        List<Color> toRemove = new List<Color>();
        foreach (Color checkColor in inSceneColors)
        {
            float dist = Vector4.Distance(color, checkColor);
            if (dist < 0.01f)
            {
                toRemove.Add(checkColor);
            }
        }
        foreach(Color removeColor in toRemove)
        {
            inSceneColors.Remove(removeColor);
        }
    }

}
