using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public Transform sheep_transform;
    public Rigidbody rigidbody;
    private int state = (int) State.Available;
    private Vector3 targetDestination;
    private float velocity = 0.5f;

    enum State { Available, Moving, Unavailable, Scared};

    private const float stepSize = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        var speed = velocity * Time.deltaTime;
        sheep_transform.position = Vector3.Lerp(sheep_transform.position, targetDestination, speed);
    }

    public IEnumerator move()
    {
        state = (int) State.Moving;

        float x_displace = Random.Range(-10.0f, 10.0f);
        float z_displace = Random.Range(-10.0f, 10.0f);
        Vector3 displacement = new Vector3(x_displace, 0f, z_displace);

        //rigidbody.AddForce(x_force, 0.0f, z_force, ForceMode.Impulse);
        targetDestination = sheep_transform.position + displacement;

        float stop_time = Random.Range(4f, 6f);

        yield return new WaitForSeconds(stop_time); // Number of seconds this sheep will wait until it can move again

        if(state == (int)State.Moving) // the state might be Unavaible if it was picked up while moving
            state = (int)State.Available;
    }

    public IEnumerator scare(Vector3 direction)//TO DO: test + change to continuous movement
    {
        state = (int)State.Scared;

        float magnitude = Mathf.Sqrt((direction.x * direction.x) + (direction.z * direction.z));

        float x_normalized = direction.x / magnitude; 
        float z_normalized = direction.z / magnitude;

        float x_intensity = 4f;
        float z_intensity = 4f;

        Vector3 displacement = new Vector3(x_intensity * x_normalized, 0.0f, z_intensity * z_normalized);

        targetDestination = sheep_transform.position + displacement;
        //rigidbody.AddForce(x_intensity * x_normalized, 0.0f, z_intensity * z_normalized, ForceMode.Impulse);

        float stop_time = 0.25f;

        yield return new WaitForSeconds(stop_time); // Number of seconds this sheep will wait until it can move again

        if (state == (int)State.Scared) // the state might be Unavaible if it was picked up while moving
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
