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
    Rigidbody rb;

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
    [SerializeField] ParticleSystem particles;

    List<ObjectSensor> hoopSensors = new List<ObjectSensor>();

    [Header("Audio + Effects")]
    [SerializeField] AudioClip collisionAudio;
    AudioSource collisionAudioSource;
    [SerializeField] float minCollisionDelay;
    [SerializeField] float minCollisionForce;
    float lastCollision;
    [Space]
    [SerializeField] AudioClip shakeAudio;
    AudioSource shakeAudioSource;
    [SerializeField] float minimumShakeSpeed;
    float xDir;
    float yDir;
    float zDir;
    Rigidbody handRigidbody;
    GameObject physicsHandObject;
    [Space]
    [SerializeField] AudioClip pickUpAudio;
    [SerializeField] bool pickUpAudioLoop;
    AudioSource pickUpAudioSource;
    [Space]
    [SerializeField] AudioClip scoreHoopAudio;
    AudioSource scoreHoopAudioSource;
    [Space]
    [SerializeField] AudioClip activateAudio;
    [SerializeField] bool activateAudioLoop;
    AudioSource activateAudioSource;
    [SerializeField] bool hapticsWhileActivated;
    [SerializeField] float hapticsIntensity;
    ActionBasedController currentController;
    bool isActivated;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        despawnTimer = 0f;
        lastCollision = 10000f;
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

        //Audio setup
        if(collisionAudio != null)
        {
            collisionAudioSource =  gameObject.AddComponent<AudioSource>();
            collisionAudioSource.playOnAwake = false;
            collisionAudioSource.clip = collisionAudio;
        }
        if(shakeAudio != null)
        {
            shakeAudioSource =  gameObject.AddComponent<AudioSource>();
            shakeAudioSource.playOnAwake = false;
            shakeAudioSource.clip = shakeAudio;
        }
        if(pickUpAudio != null)
        {
            pickUpAudioSource =  gameObject.AddComponent<AudioSource>();
            pickUpAudioSource.playOnAwake = false;
            pickUpAudioSource.clip = pickUpAudio;
            pickUpAudioSource.loop = pickUpAudioLoop;
        }
        if(scoreHoopAudio != null)
        {
            scoreHoopAudioSource =  gameObject.AddComponent<AudioSource>();
            scoreHoopAudioSource.playOnAwake = false;
            scoreHoopAudioSource.clip = scoreHoopAudio;
        }
        if(activateAudio != null)
        {
            activateAudioSource =  gameObject.AddComponent<AudioSource>();
            activateAudioSource.playOnAwake = false;
            activateAudioSource.clip = activateAudio;
            activateAudioSource.loop = activateAudioLoop;
        }
        

    }

    public void Activate()
    {
        isActivated = true;
        Audio.AttemptPlayAudio(activateAudioSource);
        
    }

    public void Deactivate()
    {
        isActivated = false;
        Audio.AttemptStopAudio(activateAudioSource);
    }

    public void PickedUp()
    {
        hasBeenHeld = true;
        isHeld = true;
        handNearby = true;
        releasedByObject = GetHoldingHand();
        currentController = releasedByObject.GetComponent<ActionBasedController>();
        if (shakeAudio != null)
        {
            physicsHandObject = releasedByObject.GetComponent<PhysicsHandReference>().physicsHand.gameObject;
            handRigidbody = physicsHandObject.GetComponent<Rigidbody>();
        }

        if (doObjectFlip)
        {
            DoObjectFlip();
        }

        Audio.AttemptPlayAudio(pickUpAudioSource);
    }


    public void Released()
    {
        isHeld = false;
        if (dynamicCollisions)
        {
            Debug.Log("changed layer");
            SetLayer(cannotTouchHandsLayer);
        }

        Audio.AttemptStopAudio(pickUpAudioSource);
            
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

    private void OnCollisionEnter(Collision other) 
    {
        if(lastCollision > minCollisionDelay)
        {
            if(rb.velocity.magnitude > minCollisionForce)
            {
                float fac = (rb.velocity.magnitude - minCollisionForce) / (minCollisionForce + 2f);

                if(Audio.AttemptPlayAudio(collisionAudioSource, fac))
                {
                    lastCollision = 0f;
                }
            }
            
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
                Audio.AttemptPlayAudio(scoreHoopAudioSource);
            }
        }
        if(spawnerMode == SpawnerType.CanSpawner)
        {
            if(canSpawner != null)
            {
                
                Color color = renderer.material.GetColor("_BaseColor");
                particles.Play();
                Audio.AttemptPlayAudio(scoreHoopAudioSource);
            }
        }
        if(spawnerMode == SpawnerType.CanSpawner)
        {
            if(singleSpawner != null)
            {
                
                Color color = renderer.material.GetColor("_BaseColor");
                particles.Play();
                Audio.AttemptPlayAudio(scoreHoopAudioSource);
            }
        }
        
        StartCoroutine(DespawnObjectWithDelay(1f));
    }

    private void Update()
    {
        if (!isHeld)
        {
            lastCollision += Time.deltaTime;
            if (doObjectDespawn)
            {

                if (despawnMode == DespawnMode.TimeAfterSpawn)
                {
                    despawnTimer += Time.deltaTime;
                }
                else if (despawnMode == DespawnMode.TimeAfterReleased)
                {
                    if (isHeld)
                    {
                        despawnTimer = 0f;
                    }
                    else
                    {
                        if (hasBeenHeld)
                        {
                            despawnTimer += Time.deltaTime;
                        }
                        else
                        {
                            despawnTimer = 0f;
                        }

                    }
                }

                else if (despawnMode == DespawnMode.TimeAfterLeaveArea)
                {
                    if (safeZone.objectsInZone.Contains(this))
                    {
                        despawnTimer = 0f;
                    }
                    else
                    {
                        if (isHeld)
                        {
                            despawnTimer = 0f;
                        }
                        else
                        {
                            despawnTimer += Time.deltaTime;
                        }

                    }

                }


                if (despawnTimer >= despawnTime && !despawning)
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
            if (shakeAudio != null)
            {

                despawnTimer = 0f;

                int directionsHaveChanged = 0;

                if (xDir != Parity(handRigidbody.velocity.x) && Mathf.Abs(xDir) < 0.1f)
                {
                    directionsHaveChanged += 1;
                }
                if (yDir != Parity(handRigidbody.velocity.y) && Mathf.Abs(yDir) < 0.1f)
                {
                    directionsHaveChanged += 1;
                }
                if (zDir != Parity(handRigidbody.velocity.z) && Mathf.Abs(zDir) < 0.1f)
                {
                    directionsHaveChanged += 1;
                }

                if (directionsHaveChanged > 0 && handRigidbody.velocity.magnitude > minimumShakeSpeed)
                {
                    Audio.AttemptPlayAudio(shakeAudioSource);
                }
                //update direction
                xDir = Parity(handRigidbody.velocity.x);
                yDir = Parity(handRigidbody.velocity.y);
                zDir = Parity(handRigidbody.velocity.z);
            }

            if (isActivated)
            {
                if (hapticsWhileActivated)
                {
                    currentController.SendHapticImpulse(hapticsIntensity, Time.deltaTime);
                }
            }

        }


    }

    float Parity(float inputFloat)
    {   
        if(inputFloat <= -0.1f)
        {
            return -1f;
        }
        else if(inputFloat >= 0.1f)
        {
            return 1f;
        }
        else
        {
            return 0f;
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
