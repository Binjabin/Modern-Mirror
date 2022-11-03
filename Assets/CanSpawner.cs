using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSpawner : MonoBehaviour
{   
    SprayCanStation[] spawnStations;
    List<Transform> spawnPoints;
    [SerializeField] GameObject canPrefab;
    [SerializeField] bool spawnCansTrigger;
    public ObjectSensor safeZone;

    [SerializeField] Gradient noCanFallbackColors;
    // Start is called before the first frame update

    public void SpawnCans()
    {
        spawnStations = GetComponentsInChildren<SprayCanStation>();
        spawnPoints = new List<Transform>();
        foreach(SprayCanStation station in spawnStations)
        {
            spawnPoints.Add(station.transform.GetChild(0));
        }

        List<Color> colorsToSpawn = GameManager.scoredColors;
        int pointIndex = 0;
        if (colorsToSpawn.Count == 0)
        {
            for(int i = 0; i < 5; i++)
            {
                float fac = Random.Range(0.0f, 1.0f);
                Color color = noCanFallbackColors.Evaluate(fac);
                GameObject spawnedCan = Instantiate(canPrefab, spawnPoints[pointIndex].position, Quaternion.identity);
                spawnedCan.GetComponent<InteractableObjectExtentions>().LinkSpawner(this, pointIndex);
                spawnedCan.GetComponent<SprayColor>().SetColor(color);

                pointIndex++;
            }
        }
        else
        {
            
            foreach (Color color in colorsToSpawn)
            {
                GameObject spawnedCan = Instantiate(canPrefab, spawnPoints[pointIndex].position, Quaternion.identity);
                Debug.Log(spawnedCan);
                spawnedCan.GetComponent<SprayColor>().SetColor(color);
                spawnedCan.GetComponent<InteractableObjectExtentions>().LinkSpawner(this, pointIndex);

                pointIndex++;
            } 
        }
        int stationIndex = 0;
        foreach(SprayCanStation station in spawnStations)
        {
            if(stationIndex >= pointIndex)
            {
                station.gameObject.SetActive(false);
            }
            stationIndex++;
        }
    }

    public void RespawnObject(int index)
    {
        List<Color> colorsToSpawn = GameManager.scoredColors;
        if (colorsToSpawn.Count == 0)
        {
            float fac = Random.Range(0.0f, 1.0f);
            Color color = noCanFallbackColors.Evaluate(fac);
        }
        else
        {
            Color color = colorsToSpawn[index];
        }
        
        GameObject spawnedCan = Instantiate(canPrefab, spawnPoints[index].position, Quaternion.identity);
        spawnedCan.GetComponent<SprayColor>().SetColor(color);
        spawnedCan.GetComponent<InteractableObjectExtentions>().LinkSpawner(this, index);
    }

    // Update is called once per frame
    void Update()
    {
        if(spawnCansTrigger)
        {
            spawnCansTrigger = false;
            SpawnCans();
        }
    }
}
