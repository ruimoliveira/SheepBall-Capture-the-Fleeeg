using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public Transform sheep_transform;
    public Rigidbody rigidbody;
    private int state = (int) State.Ready;

    enum State { Ready, Busy };

    private const float stepSize = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        //rigidbody.AddForce(new Vector3(50, 0, 0), ForceMode.Acceleration);

        /*timeUntilUpdate -= 1;

        if(timeUntilUpdate <= 0){
            sheep_transform.position = sheep_transform.position + new Vector3(stepSize,0f,0f);
            timeUntilUpdate = refreshRate;
        }*/
    }

    public void move()
    {
        Debug.Log("called move");
    }

    public int getState()
    {
        return state;
    }

}
