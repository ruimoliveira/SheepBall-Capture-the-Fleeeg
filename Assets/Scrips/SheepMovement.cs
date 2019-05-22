using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SheepMovement : NetworkBehaviour
{
    public Transform sheep_transform;
    public Rigidbody m_Rigidbody;
    private int state = (int) State.Available;
    private Vector3 targetDestination;
    private Quaternion targetRotation;

    private const float ROTATION_SPEED = 500f;
    private const float MOVING_SPEED = 5f;
    private const float SCARED_ROTATION_SPEED = 1000f;
    private const float SCARED_MOVING_SPEED = 2f;

    enum State { Available, Rotating, Moving, Waiting, Unavailable, Scared };
    
    private Animator m_animator;

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
        m_animator = GetComponentInChildren<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    int prevState = -1;

    // Update is called once per frame
    void FixedUpdate()
    {

        if (!isServer)
        {
            // NetworkLerp();
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

                sheep_transform.position = Vector3.MoveTowards(sheep_transform.position, targetDestination, MOVING_SPEED * Time.fixedDeltaTime);
                if (Vector3.Distance(sheep_transform.position, targetDestination) < 0.001f)
                    state = (int)State.Waiting;

                break;

            case (int)State.Scared:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, SCARED_ROTATION_SPEED * Time.fixedDeltaTime);

                sheep_transform.position = Vector3.MoveTowards(sheep_transform.position, targetDestination, SCARED_MOVING_SPEED * Time.fixedDeltaTime);
                
                break;
        }

        if (state == (int)State.Scared || state == (int)State.Moving)
        {
            if (prevState != 1)
            {
                m_animator.SetInteger("AnimIndex", 1);
                m_animator.SetTrigger("Next");
            }
            prevState = 1;
        }
        else
        {
            if (prevState != 0)
            {
                m_animator.SetInteger("AnimIndex", 0);
                m_animator.SetTrigger("Next");
            }
            prevState = 0;
        }

        /*if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }*/

    }
    
    public IEnumerator move()
    {
        float x_displace = Random.Range(-10.0f, 10.0f);
        float z_displace = Random.Range(-10.0f, 10.0f);
        Vector3 displacement = new Vector3(x_displace, 0f, z_displace);

        targetDestination = sheep_transform.position + displacement;
        targetRotation = Quaternion.LookRotation( sheep_transform.position - targetDestination);

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
        float magnitude = Mathf.Sqrt((direction.x * direction.x) + (direction.z * direction.z));

        float x_normalized = direction.x / magnitude; 
        float z_normalized = direction.z / magnitude;

        float x_intensity = 2f;
        float z_intensity = 2f;

        Vector3 displacement = new Vector3(x_intensity * x_normalized, 0.0f, z_intensity * z_normalized);

        targetDestination = sheep_transform.position + displacement;
        targetRotation = Quaternion.LookRotation(sheep_transform.position - targetDestination);

        state = (int)State.Scared;
    }

    public void unscare()
    {
        state = (int)State.Available;
    }
    /*
    private void OnReceiveSheepMessage(NetworkMessage _message)
    {
        SheepMovementMessage _msg = _message.ReadMessage<SheepMovementMessage>();

        //TO DO: check how manager works
        Manager.Instance.ConnectedPlayers[_msg.objectTransformName].GetComponent<SheepMovement>().ReceiveMovementMessage(_msg.objectPosition, _msg.objectRotation, _msg.time);
    }

    public void ReceiveMovementMessage(Vector3 _position, Quaternion _rotation, float _timeToLerp)
    {
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

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;
        SendMovementMessage(sheepID, transform.position, transform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart));
        canSendNetworkMovement = false;
    }

    public void SendMovementMessage(string _sheepID, Vector3 _position, Quaternion _rotation, float _timeTolerp)
    {
        SheepMovementMessage _msg = new SheepMovementMessage()
        {
            objectPosition = _position,
            objectRotation = _rotation,
            objectTransformName = _sheepID,
            time = _timeTolerp
        };

        NetworkManager.singleton.client.Send(movement_msg, _msg);
    }

    private void NetworkLerp()
    {
        if (isLerpingPosition)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            sheep_transform.position = Vector3.Lerp(lastRealPosition, realPosition, lerpPercentage);
        }

        if (isLerpingRotation)
        {
            float lerpPercentage = (Time.time - timeStartedLerping) / timeToLerp;

            sheep_transform.rotation = Quaternion.Lerp(lastRealRotation, realRotation, lerpPercentage);
        }
    }
    */
    public int getState()
    {
        return state;
    }

    public void setState(int state)
    {
        this.state = state;
    }
}