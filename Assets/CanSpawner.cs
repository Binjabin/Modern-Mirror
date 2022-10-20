using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSpawner : MonoBehaviour
{
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();
    [SerializeField] GameObject canPrefab;
    [SerializeField] bool spawnCansTrigger;

    // Start is called before the first frame update

    private void Start()
    {

    }
    void SpawnCans()
    {
        List<Color> colorsToSpawn = GameInformation.scoredColors;
        foreach (Color color in colorsToSpawn)
        {
            int pointIndex = Random.Range(0, spawnPoints.Count - 1);
            GameObject spawnedCan = Instantiate(canPrefab, spawnPoints[pointIndex]);
            spawnedCan.GetComponent<SprayColor>().SetColor(color);
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
