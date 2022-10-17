using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SprayPainter: MonoBehaviour{
    public Color paintColor;
    
    public float radius = 0.05f;
    public float strength = 1;
    public float hardness = 1;

    public float range = 5;
    private ActionBasedController controller;
    bool isPressed;
    bool isHeld;

    [SerializeField] ParticleSystem sprayEffect;
    [SerializeField] GameObject handOrigin;

    XRDirectInteractor[] hands;


    void Start(){
        //var pr = part.GetComponent<ParticleSystemRenderer>();
        //Color c = new Color(pr.material.color.r, pr.material.color.g, pr.material.color.b, .8f);
        //paintColor = c;
        hands = FindObjectsOfType<XRDirectInteractor>();
    }

    public void Activated() { isPressed = true; }
    public void Deactivated() { isPressed = false; }
    public void Selected()
    { 
        isHeld = true; 
        if (hands[0].gameObject.tag == "LeftHand" && hands[0].interactablesSelected[0].transform.gameObject == gameObject.transform.parent.gameObject)
        {
            handOrigin.transform.localEulerAngles = new Vector3(90, 0, 0);
        }
        else if (hands[1].gameObject.tag == "RightHand" && hands[1].interactablesSelected[0].transform.gameObject == gameObject.transform.parent.gameObject)
        {
            handOrigin.transform.localEulerAngles = new Vector3(-90, 0, 0);
        }
    }

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
                Physics.Raycast(transform.position, raycastDirection, out hit, range);
                Debug.DrawRay(transform.position, raycastDirection, Color.red);
                Paintable p;
                float lerpFactor = (hit.distance) / range;
                //paintColor.a = Mathf.Lerp(1, 0, lerpFactor);
                if (hit.collider != null)
                {
                    p = hit.collider.gameObject.GetComponent<Paintable>();
                    if (p != null)
                    {
                        Debug.Log("a particle paints!");
                        Vector3 pos = hit.point;
                        float paintRadius = hit.distance * radius;
                        PaintManager.instance.paint(p, pos, paintRadius, hardness, strength, paintColor);
                    }
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


    Vector3 GetRandomDirectionInCone(float radius)
    {
        Vector2 circle = Random.insideUnitCircle * radius;
        Vector3 target = transform.position + transform.forward + transform.rotation * new Vector3(circle.x, circle.y);
        Vector3 direction = Vector3.Normalize(target - transform.position);
        return direction;
    }
}