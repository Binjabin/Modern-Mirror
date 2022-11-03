using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public enum DespawnMode
{
    TimeAfterSpawn,
    TimeAfterReleased,
    TimeAfterLeaveArea,
    
}
public enum SpawnerType
{
    QueuedObjectSpawner,
    CanSpawner,
    SingleObjectSpawner
}
public class InteractableObjectExtentions : MonoBehaviour
{
    [Header("Hands")]
    public XRDirectInteractor[] hands;

    [Header("Collision Settings")]
    [SerializeField] bool dynamicCollisions;
    [SerializeField] LayerMask canTouchHandsLayer;
    [SerializeField] LayerMask cannotTouchHandsLayer;
    bool isHeld;
    bool handNearby;
    GameObject releasedByObject;

    [Header("Object Flip")]
    [SerializeField] bool doObjectFlip;
    [SerializeField] GameObject holdingPivotPoint;
    [SerializeField] Vector3 leftHandRotation;
    [SerializeField] Vector3 rightHandRotation;

    [Header("Spawning")]
    [SerializeField] bool doObjectDespawn;
    [SerializeField] bool growOnObjectSpawn;
    [SerializeField] DespawnMode despawnMode;
    [SerializeField] SpawnerType spawnerMode;
    bool hasBeenHeld;
    [SerializeField] float despawnTime;
    MeshRenderer renderer;
    bool despawning;

    float despawnTimer;
    QueuedObjectSpawner queuedObjectSpawner;
    CanSpawner canSpawner;
    SingleObjectSpawner singleSpawner;
    int canStationIndex;
    ObjectSensor safeZone;

    [Header("Hoop")]
    [SerializeField] bool isScoreable;
    [SerializeField] AudioSource scoreSound;
    [SerializeField] ParticleSystem particles;

    List<ObjectSensor> hoopSensors = new List<ObjectSensor>();


    void Start()
    {
        despawnTimer = 0f;
        despawning = false;
        hands = FindObjectsOfType<XRDirectInteractor>();
        handNearby = false;
        isHeld = false;
        hasBeenHeld = false;
        SetLayer(canTouchHandsLayer);
        GetHoopSensors();
        renderer = GetComponent<MeshRenderer>();
        if(renderer == null)
        {
            renderer = GetComponentInChildren<MeshRenderer>();
        }
        if(growOnObjectSpawn)
        {
            StartCoroutine(GrowOverTime(1f));
        }
    }

    void GetHoopSensors()
    {
        var sensors = FindObjectsOfType<ObjectSensor>();
        foreach(ObjectSensor sensor in sensors)
        {
            if(sensor.isHoop)
            {
                hoopSensors.Add(sensor);
            }
        }
    }
    // Update is called once per frame
    public void Released()
    {
        isHeld = false;
        if (dynamicCollisions)
        {
            Debug.Log("changed layer");
            SetLayer(cannotTouchHandsLayer);
        }
            
    }
    public void LinkSpawner(QueuedObjectSpawner linkThis)
    {
        queuedObjectSpawner = linkThis;
        if(despawnMode == DespawnMode.TimeAfterLeaveArea)
        {
            safeZone = queuedObjectSpawner.safeZone;
        }
    }
    public void LinkSpawner(CanSpawner linkThis, int index)
    {
        canSpawner = linkThis;
        if(despawnMode == DespawnMode.TimeAfterLeaveArea)
        {
            safeZone = canSpawner.safeZone;
            canStationIndex = index;
        }
    }
    public void LinkSpawner(SingleObjectSpawner linkThis)
    {
        singleSpawner = linkThis;
        if(despawnMode == DespawnMode.TimeAfterLeaveArea)
        {
            safeZone = singleSpawner.safeZone;
        }
    }
    public void PickedUp()
    {
        hasBeenHeld = true;
        isHeld = true;
        handNearby = true;

        if (dynamicCollisions)
        {
            releasedByObject = GetHoldingHand();
        }

        if (doObjectFlip)
        {
            DoObjectFlip();
        }

    }

    public void ExitHandProximity(GameObject hand)
    {
        if(dynamicCollisions)
        {
            Debug.Log("a hand left proximity. Checking for: " + releasedByObject + ". Event triggered by " + hand);
            if (!isHeld)
            {
                
                if (releasedByObject == hand)
                {
                    Debug.Log("most recent holding hand left proximity");
                    SetLayer(canTouchHandsLayer);
                    handNearby = false;
                }
            }
        }
    }


    //sets the layer of all child colliders
    void SetLayer(LayerMask layer)
    {
        gameObject.layer = (int)Mathf.Log(layer.value, 2);
        foreach (Transform child in transform)
        {
            child.gameObject.layer = (int)Mathf.Log(layer.value, 2);
        }
    }

    void DoObjectFlip()
    {
        //Flip object depending on hands
        GameObject currentHand = GetHoldingHand();
        if (currentHand != null)
        {
            
            if (currentHand.tag == "LeftHand")
            {
                holdingPivotPoint.transform.localEulerAngles = leftHandRotation;

            }
            else if (currentHand.tag == "RightHand")
            {
                holdingPivotPoint.transform.localEulerAngles = rightHandRotation;
            }

        }
        else
        {
            Debug.Log("object flip called with no hand found");
        }
        
    }

    void DespawnObject()
    {
        if(spawnerMode == SpawnerType.QueuedObjectSpawner)
        {
            if(queuedObjectSpawner != null)
            {
                Color color = renderer.material.GetColor("_BaseColor");
                queuedObjectSpawner.RemoveColor(color);
            }
        }
        if(spawnerMode == SpawnerType.CanSpawner)
        {
            if(canSpawner != null)
            {
                canSpawner.RespawnObject(canStationIndex);
            }
        }
        if(spawnerMode == SpawnerType.SingleObjectSpawner)
        {
            if(singleSpawner != null)
            {
                singleSpawner.RespawnObject();
            }
        }
        Destroy(gameObject);
    }

    IEnumerator DespawnObjectWithDelay(float secs)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;
        while(elapsedTime < secs)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, elapsedTime / secs);
            yield return null;
        }
        DespawnObject();
        
    }

    IEnumerator GrowOverTime(float secs)
    {
        float elapsedTime = 0f;
        Vector3 initialScale = transform.localScale;
        while(elapsedTime < secs)
        {
            elapsedTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, initialScale, elapsedTime / secs);
            yield return null;
        }
        
    }

    void ScoreObject()
    {
        if(spawnerMode == SpawnerType.QueuedObjectSpawner)
        {
            if(queuedObjectSpawner != null)
            {
                Color color = renderer.material.GetColor("_BaseColor");
                queuedObjectSpawner.ScoreColor(color);
                particles.Play();
                scoreSound.Play();
            }
        }
        if(spawnerMode == SpawnerType.CanSpawner)
        {
            if(canSpawner != null)
            {
                
                Color color = renderer.material.GetColor("_BaseColor");
                particles.Play();
                scoreSound.Play();
            }
        }
        if(spawnerMode == SpawnerType.CanSpawner)
        {
            if(singleSpawner != null)
            {
                
                Color color = renderer.material.GetColor("_BaseColor");
                particles.Play();
                scoreSound.Play();
            }
        }
        
        StartCoroutine(DespawnObjectWithDelay(1f));
    }

    private void Update()
    {
        if (!isHeld)
        {
            
            if(doObjectDespawn)
            {
                
                if(despawnMode == DespawnMode.TimeAfterSpawn)
                {
                    despawnTimer += Time.deltaTime;
                }
                else if(despawnMode == DespawnMode.TimeAfterReleased)
                {
                    if(isHeld)
                    {
                        despawnTimer = 0f;
                    }
                    else
                    {
                        if(hasBeenHeld)
                        {
                            despawnTimer += Time.deltaTime;
                        }
                        else
                        {
                            despawnTimer = 0f;
                        }
                        
                    }
                }
                
                else if(despawnMode == DespawnMode.TimeAfterLeaveArea)
                {
                    if(safeZone.objectsInZone.Contains(this))
                    {
                        despawnTimer = 0f;
                    }
                    else
                    {
                        if(isHeld)
                        {
                            despawnTimer = 0f;
                        }
                        else
                        {
                            despawnTimer += Time.deltaTime;
                        }
                        
                    }
                    
                }


                if(despawnTimer >= despawnTime && !despawning)
                {
                    StartCoroutine(DespawnObjectWithDelay(0.5f));
                    despawning = true;
                }
            }
            
            if (!handNearby)
            {
                
                SetLayer(canTouchHandsLayer);
            }
        }
        else
        {
            despawnTimer = 0f;
        }

    }

    
    GameObject GetHoldingHand()
    {
        GameObject holdingHand = null;
        if (isHeld)
        {
            for (int i = 0; i < hands.Length; i++)
            {
                Debug.Log(hands[i]);
                if(hands[i].interactablesSelected.Count > 0)
                {
                    if (hands[i].interactablesSelected[0].transform.gameObject == gameObject)
                    {
                        Debug.Log(hands[i].interactablesSelected[0].transform.gameObject);
                        holdingHand = hands[i].gameObject;
                    }

                }
            }
            return holdingHand;
        }
        else
        {
            return null;
        }
    }

    bool IsScored()
    {
        bool isScored = true;
        foreach(ObjectSensor sensor in hoopSensors)
        {
            if(!sensor.objectsInZone.Contains(this))
            {
                isScored = false;
                Debug.Log("Not all sensors contain ball!");
            }
        }
        return isScored;
    }

    

    public void EnteredSensor(ObjectSensor sensor)
    {
        if(isScoreable && sensor.isHoop)
        {
            if(IsScored())
            {
                Debug.Log("Score a hoop!");
                ScoreObject();
            }
        }
    }
    
    public void ExitedSensor(ObjectSensor sensor)
    {

    }


}
