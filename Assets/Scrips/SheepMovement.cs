using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public Transform sheep_transform;
    public Rigidbody rigidbody;
    private int state = (int) State.Available;

    enum State { Available, Moving, Unavailable, Scared};

    private const float stepSize = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }

    public IEnumerator move()
    {
        state = (int) State.Moving;

        float x_force = Random.Range(-15.0f, 15.0f);
        float z_force = Random.Range(-15.0f, 15.0f);
        float stop_time = Random.Range(2f, 4f);

        rigidbody.AddForce(x_force, 0.0f, z_force, ForceMode.Impulse);

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

        float stop_time = 0.25f;

        rigidbody.AddForce(x_intensity * x_normalized, 0.0f, z_intensity * z_normalized, ForceMode.Impulse);

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
