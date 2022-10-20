using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionPaint : MonoBehaviour
{
    public Color paintColor;
    public float radius = 0.05f;
    public float strength = 1;
    public float hardness = 1;

    private void OnCollisionStay(Collision other) {
        Paintable p = other.collider.GetComponent<Paintable>();
        if(p != null){
            
            Vector3 pos = other.contacts[0].point;
            PaintManager.instance.paint(p, pos, radius, hardness, strength, paintColor);
        }
    }
}
