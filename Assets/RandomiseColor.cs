using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomiseColor : MonoBehaviour
{
    [SerializeField] Gradient colorRange;
    [SerializeField] GameObject cap;
    [SerializeField] Material defaultCapMaterial;
    // Start is called before the first frame update
    void Start()
    {
        float fac = Random.Range(0f, 1f);
        Color randomColor = colorRange.Evaluate(fac);
        cap.GetComponent<MeshRenderer>().material = defaultCapMaterial;
        cap.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", randomColor);
        GetComponentInChildren<SprayPainter>().paintColor = randomColor;
        GetComponentInChildren<SprayPainter>().paintColor.a = 0.9f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
