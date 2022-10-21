using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSpawner : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] GameObject canPrefab;
    [SerializeField] bool spawnCansTrigger;


    [SerializeField] Gradient noCanFallbackColors;
    // Start is called before the first frame update

    private void Start()
    {

    }
    public void SpawnCans()
    {
        List<Color> colorsToSpawn = GameManager.scoredColors;
        
        foreach (Color color in colorsToSpawn)
        {
            int pointIndex = Random.Range(0, spawnPoints.Count - 1);
            GameObject spawnedCan = Instantiate(canPrefab, spawnPoints[pointIndex]);
            spawnedCan.GetComponent<SprayColor>().SetColor(color);
        }

        if (colorsToSpawn.Count == 0)
        {
            for(int i = 0; i < 5; i++)
            {
                float fac = Random.Range(0.0f, 1.0f);
                Color color = noCanFallbackColors.Evaluate(fac);

                int pointIndex = Random.Range(0, spawnPoints.Count - 1);
                GameObject spawnedCan = Instantiate(canPrefab, spawnPoints[pointIndex]);
                spawnedCan.GetComponent<SprayColor>().SetColor(color);
                
            }
        }
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
