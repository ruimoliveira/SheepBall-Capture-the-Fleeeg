using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public Transform sheep_transform;
    public Rigidbody rigidbody;
    private int state = (int) State.Available;
    private Vector3 targetDestination;
    private Quaternion targetRotation;
    private float rotationSpeed = 50f;
    private float movingSpeed = 5f;
    private float scaredRotationSpeed= 100f;
    private float scaredMovingSpeed = 2f;

    enum State { Available, Rotating, Moving, Waiting, Unavailable, Scared };

    // Start is called before the first frame update
    void Awake()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (state == (int)State.Rotating)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            if (angle == 0)
                state = (int)State.Moving;
        }

        if (state == (int)State.Moving)
        {
            sheep_transform.position = Vector3.MoveTowards(sheep_transform.position, targetDestination, movingSpeed * Time.fixedDeltaTime);
            if (Vector3.Distance(sheep_transform.position, targetDestination) < 0.001f)
                state = (int)State.Waiting;
        }

        if (state == (int)State.Scared)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, scaredRotationSpeed * Time.fixedDeltaTime);
            sheep_transform.position = Vector3.MoveTowards(sheep_transform.position, targetDestination, scaredMovingSpeed * Time.fixedDeltaTime);
        }
    }

    public IEnumerator move()
    {
        float x_displace = Random.Range(-10.0f, 10.0f);
        float z_displace = Random.Range(-10.0f, 10.0f);
        Vector3 displacement = new Vector3(x_displace, 0f, z_displace);

        targetDestination = sheep_transform.position + displacement;
        targetRotation = Quaternion.LookRotation(targetDestination - sheep_transform.position);

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
        targetRotation = Quaternion.LookRotation(targetDestination - sheep_transform.position);

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
}
