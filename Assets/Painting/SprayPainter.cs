using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SprayPainter: MonoBehaviour{
    public Color paintColor;
    [SerializeField] bool variesWithDistance = true;
    public float radius = 0.05f;
    public float strength = 1;
    public float hardness = 1;

    public float range = 5;
    private ActionBasedController controller;
    bool isPressed;
    bool isHeld;
    [SerializeField] LayerMask sprayMask;
    [SerializeField] ParticleSystem sprayEffect;



    void Start(){
       
    }

    public void Activated() { isPressed = true; }
    public void Deactivated() { isPressed = false; }
    public void Selected(){ isHeld = true; }
    public void Deselected() { isPressed = false; isHeld = false; }


    void Update()
    {
        if(isHeld)
        {
            if (isPressed)
            {
                if(!sprayEffect.isPlaying)
                {
                    sprayEffect.Play();
                    Debug.Log("start spraying");
                }
                
                RaycastHit hit;
                Vector3 raycastDirection = transform.forward;
                //GetRandomDirectionInCone(spraySpread);
                Physics.Raycast(transform.position, raycastDirection, out hit, Mathf.Infinity, sprayMask, QueryTriggerInteraction.Ignore);
                Debug.DrawRay(transform.position, raycastDirection, Color.red);
                
                Paintable p;
                float lerpFactor = (hit.distance) / range;
                
                //paintColor.a = Mathf.Lerp(1, 0, lerpFactor);
                if (hit.collider != null && hit.distance <= range)
                {
                    Debug.Log(Mathf.Round(hit.distance * 100f) / 100f + "/" + range + " : " + hit.collider.gameObject);
                    p = hit.collider.gameObject.GetComponent<Paintable>();
                    if (p != null)
                    {
                        Vector3 pos = hit.point;
                        float paintRadius;
                        if(variesWithDistance)
                        {
                            paintRadius = hit.distance * radius;
                        }
                        else
                        {
                            paintRadius = radius;
                        }
                        if(p.totalFill == true)
                        {
                            paintRadius = 10000;
                        }
                        PaintManager.instance.paint(p, pos, paintRadius, hardness, strength, paintColor);
                    }
                }
                else
                {
                    Debug.Log("Hit nothing!");
                }
            }
            else
            {
                sprayEffect.Stop();
            }
        }
        else
        {
            sprayEffect.Stop();
        }
        
        
    }
}