using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepMovement : MonoBehaviour
{
    public Transform sheep_transform;
    private const int refreshRate = 10;
    private const float stepSize = 0.25f;
    private int timeUntilUpdate = refreshRate;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeUntilUpdate -= 1;

        if(timeUntilUpdate <= 0){
            sheep_transform.position = sheep_transform.position + new Vector3(stepSize,0f,0f);
            timeUntilUpdate = refreshRate;
        }
    }
}
