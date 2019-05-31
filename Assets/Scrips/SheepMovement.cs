using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public Transform sheep_transform;
    public Rigidbody m_Rigidbody;
    private int state = (int) State.Available;
    private Vector3 targetDestination;
    private Quaternion targetRotation;
    private float rotationSpeed = 500f;
    private float movingSpeed = 5f;
    private float scaredRotationSpeed= 1000f;
    private float scaredMovingSpeed = 2f;

    enum State {
        Available,
        Rotating,
        Moving,
        Waiting,
        Unavailable,
        Scared
    };
    
    private Animator m_animator;

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
        switch (state)
        {
            case (int)State.Rotating:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
                float angle = Quaternion.Angle(transform.rotation, targetRotation);
                if (angle == 0)
                    state = (int)State.Moving;
                
                break;

            case (int)State.Moving:

                sheep_transform.position = Vector3.MoveTowards(sheep_transform.position, targetDestination, movingSpeed * Time.fixedDeltaTime);
                if (Vector3.Distance(sheep_transform.position, targetDestination) < 0.001f)
                    state = (int)State.Waiting;

                break;

            case (int)State.Scared:
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, scaredRotationSpeed * Time.fixedDeltaTime);

                sheep_transform.position = Vector3.MoveTowards(sheep_transform.position, targetDestination, scaredMovingSpeed * Time.fixedDeltaTime);
                
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

    public int getState()
    {
        return state;
    }

    public void setState(int state)
    {
        this.state = state;
    }

    public void setAvailable()
    {
        this.state = (int)State.Available;
    }

    public void setUnavailable()
    {
        this.state = (int)State.Unavailable;
    }
}
