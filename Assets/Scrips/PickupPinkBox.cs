using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupPinkBox : MonoBehaviour
{
    int colligingWithBoxSem = 0;
    bool pickedUpBox = false;
    Stack<GameObject> pinkBoxCollided;
    Stack<GameObject> pinkBoxStackPickedUp;
    Transform pinkBoxFather;
    string pinkBoxTag = "PinkBox";

    // Start is called before the first frame update
    void Start()
    {

        GameObject[] allPinkBoxes = GameObject.FindGameObjectsWithTag(pinkBoxTag);
        foreach (GameObject pinkboxAux in allPinkBoxes)
        {
            Physics.IgnoreCollision(pinkboxAux.GetComponent<Collider>(), GetComponent<Collider>());
        }

        pinkBoxStackPickedUp = new Stack<GameObject>();
        pinkBoxCollided = new Stack<GameObject>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(colligingWithBoxSem + "TRIGGER COLLIDED WITH " + other.tag);
        if (pinkBoxTag == other.tag)
        {
            pinkBoxCollided.Push(other.gameObject);
            pinkBoxFather = other.gameObject.transform.parent;
            colligingWithBoxSem ++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(colligingWithBoxSem + "TRIGGER EXITED WITH " + other.tag);
        if (pinkBoxTag == other.tag)
        {
            colligingWithBoxSem--;
            pinkBoxCollided.Pop();
        }
    }

    private void dropBox()
    {
        //TODO: Speed Guy Back Up
        GameObject pinkBoxDrop = pinkBoxStackPickedUp.Pop();
        Rigidbody rb = pinkBoxDrop.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
        rb.useGravity = true;
        pinkBoxDrop.transform.parent = pinkBoxFather;
    }

    private void pickupBox()
    {
        //TODO: Slow Guy Down
        GameObject pinkBox = pinkBoxCollided.Pop();
        pinkBoxCollided.Push(pinkBox);
        pinkBoxStackPickedUp.Push(pinkBox);
        Rigidbody rb = pinkBox.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        pinkBox.transform.parent = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (0 < colligingWithBoxSem && pinkBoxStackPickedUp.ToArray().Length < 4)
            {
                pickupBox();
            }
            else
                if (pinkBoxStackPickedUp.ToArray().Length > 0)
                {
                   dropBox();
                }
        }

        int index = 0;
        GameObject[] arrPinkBoxes = pinkBoxStackPickedUp.ToArray();
        foreach (GameObject pB in arrPinkBoxes)
        {
            Vector3 newV = gameObject.transform.position;
            newV.y += 1;
            switch (index++)
            {
                case 0:
                    newV.y += 2;
                    break;
                case 1:
                    newV.x += 1.5f;
                    break;
                case 2:
                    newV.x -= 1.5f;
                    break;
            }
            pB.transform.position = newV;
        }
    }
}
