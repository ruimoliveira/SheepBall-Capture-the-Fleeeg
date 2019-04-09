using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPinkBox : MonoBehaviour
{
    bool colligingWithBox = false;
    bool pickedUpBox = false;
    GameObject pinkBox;
    Transform pinkBoxFather;

    // Start is called before the first frame update
    void Start()
    {
        pinkBox = GameObject.FindWithTag("PinkBox");
        pinkBoxFather = pinkBox.transform.parent;
        Physics.IgnoreCollision(pinkBox.GetComponent<Collider>(), GetComponent<Collider>());
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER COLLEDED WITH " + other.tag);
        if(pinkBox.tag == other.tag)
            colligingWithBox = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TRIGGER EXITED WITH " + other.tag);
        if (pinkBox.tag == other.tag)
            colligingWithBox = false;
    }


    private void moveBoxToTopOfHead()
    {
        Rigidbody rb = pinkBox.GetComponent<Rigidbody>();
        if (pickedUpBox)
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.useGravity = true;
            pinkBox.transform.parent = pinkBoxFather;
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            Vector3 ls = pinkBox.transform.localScale;
            pinkBox.transform.parent = gameObject.transform;
        }
        pickedUpBox = !pickedUpBox;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && colligingWithBox)
        {
            moveBoxToTopOfHead();
        }

        if (pickedUpBox)
        {
            pinkBox.transform.position = gameObject.transform.position;
        }
    }
}
