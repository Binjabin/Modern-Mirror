using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SprayColor : MonoBehaviour
{
    
    [SerializeField] GameObject cap;
    [SerializeField] GameObject sleave;
    [SerializeField] Material defaultCapMaterial;
    [SerializeField] GameObject sprayEffect;
    // Start is called before the first frame update

    // Update is called once per frame
    public void SetColor(Color color)
    {
        
        cap.GetComponent<MeshRenderer>().material = defaultCapMaterial;
        cap.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", color);
        sleave.GetComponent<MeshRenderer>().materials[1] = defaultCapMaterial;
        sleave.GetComponent<MeshRenderer>().materials[1].SetColor("_BaseColor", color);
        GetComponentInChildren<SprayPainter>().paintColor = color;
        sprayEffect.GetComponent<ParticleSystemRenderer>().material.SetColor("_BaseColor", color);
    }    
}
