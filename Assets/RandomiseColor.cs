using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseColor : MonoBehaviour
{
    [SerializeField] Gradient colorRange;
    [SerializeField] GameObject cap;
    [SerializeField] GameObject sleave;
    [SerializeField] Material defaultCapMaterial;
    [SerializeField] GameObject sprayEffect;
    // Start is called before the first frame update
    void Start()
    {
        float fac = Random.Range(0f, 1f);
        Color randomColor = colorRange.Evaluate(fac);
        cap.GetComponent<MeshRenderer>().material = defaultCapMaterial;
        cap.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randomColor);
        sleave.GetComponent<MeshRenderer>().materials[1] = defaultCapMaterial;
        sleave.GetComponent<MeshRenderer>().materials[1].SetColor("_BaseColor", randomColor);
        GetComponentInChildren<SprayPainter>().paintColor = randomColor;
        sprayEffect.GetComponent<ParticleSystemRenderer>().material.SetColor("_BaseColor", randomColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
