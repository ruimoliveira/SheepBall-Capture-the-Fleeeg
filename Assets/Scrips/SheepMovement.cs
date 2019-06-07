using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SheepAnimationState;

public class SheepMovement : NetworkMessageHandler
{
    private Rigidbody m_Rigidbody;
    private Vector3 targetDestination;
    private Quaternion targetRotation;
    private int state = (int)State.Available;
    private string pickedUpBy;
    private IAnimState animState;
    private Animator m_animator;
    // private TerrainData m_arena;
    private GameObject[] baseWalls;

    private const float ROTATION_SPEED = 500f;
    private const float MOVING_SPEED = 5f;
    private const float SCARED_ROTATION_SPEED = 1000f;
    private const float SCARED_MOVING_SPEED = 2f;
    private const float sheepFeetFromFloor = 2;

    [Header("Sheep Properties")]
    public string sheepID;

    [Header("Sheep Movement Properties")]
    public bool canSendNetworkMovement;
    public float speed;
    public float networkSendRate = 5;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;

    [Header("Lerping Properties")]
    public bool isLerpingPosition;
    public bool isLerpingRotation;
    public Vector3 realPosition;
    public Quaternion realRotation;
    public Vector3 lastRealPosition;
    public Quaternion lastRealRotation;
    public float timeStartedLerping;
    public float timeToLerp;

    // Start is called before the first frame update
    void Start()
    {
        sheepID = "sheep" + GetComponent<NetworkIdentity>().netId.ToString();
        transform.name = sheepID;

        m_Rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponentInChildren<Animator>();
        animState = new Iddle(ref m_animator);
        // m_arena = GameObject.FindGameObjectWithTag("Arena").GetComponent<Terrain>().terrainData;
        baseWalls = GameObject.FindGameObjectsWithTag(Constants.BASE_WALL_TAG);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isServer)
        {
            NetworkLerp();
            return;
        }
        else if (state == (int)State.Unavailable){ //servidor so faz lerp se pickedup sheeps (unica situacao em que cliente tem autoridade sobre elas)
            NetworkLerp();
            return;
        }

        switch (state)
        {
            case (int)State.Rotating:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, ROTATION_SPEED * Time.fixedDeltaTime);
                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                if (angle == 0)
                    state = (int)State.Moving;

                break;

            case (int)State.Moving:

                transform.position = Vector3.MoveTowards(transform.position, targetDestination, MOVING_SPEED * Time.fixedDeltaTime);
                if (Vector3.Distance(transform.position, targetDestination) < 0.01f)
                    state = (int)State.Waiting;

                break;

            case (int)State.Scared:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, SCARED_ROTATION_SPEED * Time.fixedDeltaTime);
                transform.position = Vector3.MoveTowards(transform.position, targetDestination, SCARED_MOVING_SPEED * Time.fixedDeltaTime);
                
                break;

            case (int)State.Flying:
                // TO DO: change to detect collision with ground and then check if sheep is immobile for a few frames
                // so that sheep is not available immediatly as it touches the ground)
                // (other alternative is to immediatly stop the sheep as it touches the ground - this eliminates the sheep's sliding,
                // and would be more coherent with the UI trajectory and target)
                Debug.Log("Flying: " + transform.position.y);
                if (transform.position.y < 0.51)
                {
                    Debug.Log("JUST LANDED :D");
                    state = (int)State.Available;
                    // IAnimState animState = new Iddle(ref m_animator);
                    // SetAnimState(animState);
                    sheepCollideWithBases(transform.gameObject, true);
                }
                /*
                 * 
                if ((int)(transform.position.y*10) <= (int)(getHeightOfTerrainAt() + sheepFeetFromFloor) * 10)
                {
                    Debug.Log("SHEEP: " + (transform.position.y) + " TERRAIN: " + ((getHeightOfTerrainAt() + sheepFeetFromFloor)));
                    Debug.Log("TODO: DEBUG THIS");
                    state = (int)State.Available;
                }
                */
                break;
        }

        animState = animState.next(state);

        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        int anim_index = m_animator.GetInteger("Index");
        SendSheepMovementMessage(sheepID, transform.position, transform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart), anim_index);
        canSendNetworkMovement = false;
    }

    public void SendSheepMovementMessage(string _sheepID, Vector3 _position, Quaternion _rotation, float _timeTolerp, int anim_index)
    {
        SheepMovementMessage _msg = new SheepMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            objectTransformName = _sheepID,
            time = _timeTolerp,
            objectAnimation = anim_index
        };

        //NetworkServer.SendToAll(sheep_movement_msg, _msg);
        NetworkManager.singleton.client.Send(sheep_movement_msg, _msg);
    }

    float getHeightOfTerrainAt()
    {
        return Terrain.activeTerrain.SampleHeight(transform.position);
    }
    
    public IEnumerator move()
    {
        float x_displace = Random.Range(-10.0f, 10.0f);
        float z_displace = Random.Range(-10.0f, 10.0f);
        Vector3 displacement = new Vector3(x_displace, 0f, z_displace);

        targetDestination = transform.position + displacement;
        targetRotation = Quaternion.LookRotation(transform.position - targetDestination);

        state = (int)State.Rotating;
        yield return new WaitUntil(() => state == (int)State.Moving); //wait until rotation finishes

        state = (int)State.Moving;
        yield return new WaitUntil(() => state == (int)State.Waiting); //wait until movement finishes

        float stop_time = Random.Range(4f, 6f);
        yield return new WaitForSeconds(stop_time); // Number of seconds this sheep will wait after it completed its movement, before being able to move again
        if (state == (int)State.Waiting) // the state might be Unavaible if it was picked up while moving
            state = (int)State.Available;
    }

    public void scare(Vector3 direction)//TO DO: test + change to continuous movement
    {
        if (state == (int)State.Flying)
            return;

        float magnitude = Mathf.Sqrt((direction.x * direction.x) + (direction.z * direction.z));

        float x_normalized = direction.x / magnitude;
        float z_normalized = direction.z / magnitude;

        float x_intensity = 2f;
        float z_intensity = 2f;

        Vector3 displacement = new Vector3(x_intensity * x_normalized, 0.0f, z_intensity * z_normalized);

        targetDestination = transform.position + displacement;
        targetRotation = Quaternion.LookRotation(transform.position - targetDestination);

        state = (int)State.Scared;
    }

    public void unscare()
    {
        state = (int)State.Available;
    }

    public void localMove(Vector3 _position, Quaternion _rotation, float _timeToLerp, int anim_index)
    {
        int previous_anim_index = m_animator.GetInteger("Index");
        if(previous_anim_index != anim_index)
        {
            m_animator.SetInteger("Index", anim_index);
            m_animator.SetTrigger("Next");
        }

        lastRealPosition = realPosition;
        lastRealRotation = realRotation;
        realPosition = _position;
        realRotation = _rotation;
        timeToLerp = _timeToLerp;

        if (realPosition != transform.position)
        {
            isLerpingPosition = true;
        }

        if (realRotation.eulerAngles != transform.rotation.eulerAngles)
        {
            isLerpingRotation = true;
        }

        timeStartedLerping = Time.time;
    }

    private void NetworkLerp()
    {
        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            transform.rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }
    
    public int getState()
    {
        return state;
    }

    public void setState(int state)
    {
        this.state = state;
    }

    public string getPickedUpBy()
    {
        return pickedUpBy;
    }

    public void setPickedUpBy(string pickedUpBy)
    {
        this.pickedUpBy = pickedUpBy;
    }

    public void setAvailable()
    {
        this.state = (int)State.Available;
    }

    public void setFlying()
    {
        this.state = (int)State.Flying;
    }

    public void setUnavailable()
    {
        this.state = (int)State.Unavailable;
    }

    public void SetAnimState(IAnimState animStateArg)
    {
        animState = animStateArg;
    }

    private void sheepCollideWithBases(GameObject sheep, bool ignore)
    {
        foreach (GameObject wall in baseWalls)
        {
            Physics.IgnoreCollision(sheep.GetComponent<Collider>(), wall.GetComponent<Collider>(), ignore);
        }
    }
}