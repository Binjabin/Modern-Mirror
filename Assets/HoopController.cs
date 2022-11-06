using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    int hoopsScored;
    [SerializeField] List<HoopKeyframeInfo> keyframes = new List<HoopKeyframeInfo>();
    bool isMovingTarget;
    Vector3 currentTargetLocation;
    Vector3 currentTargetRotation;
    [SerializeField] float toNextKeyframeSpeed = 10f;
    float distanceToTarget;
    bool hasReachedStartPoint;
    float currentMoveSpeed;
    bool moveForwards;
    [SerializeField] GameObject hoopScalePoint;
    Vector3 targetScale;
    Vector3 startScale;
    float timeSinceChange;
    // Start is called before the first frame update
    void Start()
    {
        currentTargetLocation = transform.localPosition;
        currentTargetRotation = transform.eulerAngles;
        startScale = hoopScalePoint.transform.localScale;
        hoopsScored = 0;
        hasReachedStartPoint = true;
        ToKeyframe(0);
    }

    // Update is called once per frame
    void Update()
    {

        distanceToTarget = Vector3.Distance(currentTargetLocation, transform.position);
        Debug.Log(distanceToTarget);
        
        if(timeSinceChange < 2f)
        {
            hoopScalePoint.transform.localScale = Vector3.Lerp(startScale, targetScale, timeSinceChange/2f);
        }
        else
        {
            hoopScalePoint.transform.localScale = targetScale;
        }
        
        if(distanceToTarget <= 0.01f)
        {
            hasReachedStartPoint = true;
            currentMoveSpeed = keyframes[hoopsScored].moveSpeed;
        }
        if(isMovingTarget)
        {
            if(distanceToTarget <= 0.01f)
            {
                if(moveForwards)
                {
                    moveForwards = false;
                    currentTargetLocation = keyframes[hoopsScored].endLocation;
                }
                else
                {
                    moveForwards = true;
                    currentTargetLocation = keyframes[hoopsScored].startLocation;
                }
            }
            else
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentTargetLocation, Time.deltaTime * toNextKeyframeSpeed);
            }
        }
        else
        {
            if(distanceToTarget <= 0.01f)
            {
                //at target
            }
            else
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentTargetLocation, Time.deltaTime * toNextKeyframeSpeed);
            }
        }
        timeSinceChange += Time.deltaTime;
    }

    void ToKeyframe(int frame)
    {
        timeSinceChange = 0f;
        currentTargetLocation = keyframes[frame].startLocation;
        if(currentTargetLocation == keyframes[frame].endLocation)
        {
            isMovingTarget = false;
        }
        else
        {
            isMovingTarget = true;
        }
        hasReachedStartPoint = false;
        moveForwards = true;
        currentMoveSpeed = toNextKeyframeSpeed;
        float scale = keyframes[frame].hoopSize;
        targetScale = new Vector3(scale, scale, scale); 
        startScale = hoopScalePoint.transform.localScale;
        Debug.Log("To keyframe " + frame + " which is a " + isMovingTarget + "");

    }

    public void HoopScored()
    {
        hoopsScored++;
        if(hoopsScored >= keyframes.Count)
        {
            hoopsScored = keyframes.Count;
        }   
        ToKeyframe(hoopsScored);
    }
}
