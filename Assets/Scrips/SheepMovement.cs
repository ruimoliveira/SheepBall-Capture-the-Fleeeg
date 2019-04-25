using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public Transform sheep_transform;
    public Rigidbody rigidbody;
    private int state = (int) State.Available;

    enum State { Available, Moving, Unavailable };

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
        Debug.Log("called move " + this);

        state = (int) State.Moving;

        float x_force = Random.Range(-15.0f, 15.0f);
        float z_force = Random.Range(-15.0f, 15.0f);
        float stop_time = Random.Range(2f, 4f);

        rigidbody.AddForce(x_force, 0.0f, z_force, ForceMode.Impulse);

        yield return new WaitForSeconds(stop_time); // Number of seconds this sheep will wait until it can move again

        state = (int) State.Available;
    }

    public int getState()
    {
        return state;
    }
}
