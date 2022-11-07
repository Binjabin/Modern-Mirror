using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    int hoopsScored;
    [SerializeField] List<HoopKeyframeInfo> keyframes = new List<HoopKeyframeInfo>();
    bool isMovingTarget;
    Vector3 currentTargetLocation;
    float currentTargetRotation;
    
    bool hasReachedStartPoint;
    float currentMoveSpeed;
    bool moveForwards;
    [SerializeField] GameObject hoopScalePoint;
    
    Vector3 targetScale;
    Vector3 startScale;
    Vector3 startPosition;
    Vector3 rotationOffset;
    float timeSinceChange;
    float startRotation;
    float startHoopOffset;

    //hoop parent
    [SerializeField] GameObject hoopParent;
    [SerializeField] float maxHoopOffsetDistance;
    Vector3 hoopParentStartPosition;


    void Start()
    {
        currentTargetLocation = transform.localPosition;
        currentTargetRotation = transform.localEulerAngles.y;
        startScale = hoopScalePoint.transform.localScale;
        hoopParentStartPosition = hoopParent.transform.localPosition;
        hoopsScored = 0;
        rotationOffset = transform.localEulerAngles;
        hasReachedStartPoint = true;
        ToKeyframe(0);
    }

    // Update is called once per frame
    void Update()
    {
        float x;
        if (timeSinceChange <= 1f)
        {
            hoopScalePoint.transform.localScale = Vector3.Lerp(startScale, targetScale, timeSinceChange);
            hoopScalePoint.transform.localScale = targetScale;
            x = Mathf.Lerp(startHoopOffset, 0f, timeSinceChange);
            hoopParent.transform.localPosition = hoopParentStartPosition + new Vector3(x, 0f, 0f);
            transform.localPosition = Vector3.Lerp(startPosition, currentTargetLocation, timeSinceChange);
            float yRot = Mathf.Lerp(startRotation, currentTargetRotation, timeSinceChange);
            transform.localEulerAngles = new Vector3(0f, yRot, 0f) + rotationOffset;

        }
        else
        {
            hoopScalePoint.transform.localScale = targetScale;
            x = Mathf.Sin((timeSinceChange-1f) * currentMoveSpeed) * maxHoopOffsetDistance;
            hoopParent.transform.localPosition = hoopParentStartPosition + new Vector3(x, 0f, 0f);
            transform.localPosition = currentTargetLocation;
            transform.localEulerAngles = new Vector3(0f, currentTargetRotation, 0f) + rotationOffset;
        }


        

        timeSinceChange += Time.deltaTime;
    }

    void ToKeyframe(int frame)
    {
        timeSinceChange = 0f;
        startRotation = currentTargetRotation;
        currentTargetLocation = keyframes[frame].location;
        currentTargetRotation = keyframes[frame].rotation;
        hasReachedStartPoint = false;
        moveForwards = true;
        currentMoveSpeed = keyframes[hoopsScored].moveSpeed;
        float scale = keyframes[frame].hoopSize;
        targetScale = new Vector3(scale, scale, scale); 
        startScale = hoopScalePoint.transform.localScale;
        startHoopOffset = hoopParent.transform.localPosition.x;
        startPosition = transform.localPosition;
        
        Debug.Log("To keyframe " + frame);

    }

    public void HoopScored()
    {
        hoopsScored++;
        if(hoopsScored >= keyframes.Count)
        {
            hoopsScored = keyframes.Count - 1;
        }   
        ToKeyframe(hoopsScored);
    }
}
