using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PickupPinkBox : MonoBehaviour
{
    int colligingWithBoxSem = 0;
    Stack<GameObject> pinkBoxCollided;
    Stack<GameObject> pinkBoxStackPickedUp;
    string pinkBoxTag = "PinkBox";
    ThirdPersonUserControl userControls;

    enum State { Available, Moving, Unavailable, Scared };

    public Stack<GameObject> getCubeStack()
    {
        return pinkBoxStackPickedUp;
    }

    private void Awake()
    {
        userControls = GetComponent<ThirdPersonUserControl>();
        pinkBoxStackPickedUp = new Stack<GameObject>();
        pinkBoxCollided = new Stack<GameObject>();
    }

    void Start()
    {
        GameObject[] allPinkBoxes = GameObject.FindGameObjectsWithTag(pinkBoxTag);
        foreach (GameObject pinkboxAux in allPinkBoxes)
        {
            Physics.IgnoreCollision(pinkboxAux.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(colligingWithBoxSem + "TRIGGER COLLIDED WITH " + other.tag);
        if (pinkBoxTag == other.tag)
        {
            var sheep = other.gameObject;
            pinkBoxCollided.Push(sheep);

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

    private void changePlayerSpeed()
    {
        switch (pinkBoxStackPickedUp.ToArray().Length)
        {
            case 0:
                userControls.changeWalkSpeed(1f);
                break;
            case 1:
                userControls.changeWalkSpeed(1f);
                break;
            case 2:
                userControls.changeWalkSpeed(0.85f);
                break;
            case 3:
                userControls.changeWalkSpeed(0.65f);
                break;
        }
    }   


    public GameObject removePinkCube()
    {
        if (pinkBoxStackPickedUp.ToArray().Length == 0)
            return null;

        GameObject pinkBoxDrop = pinkBoxStackPickedUp.Pop();
        pinkBoxDrop.GetComponent<SheepMovement>().setState((int)State.Available);
        changePlayerSpeed();

        Rigidbody rb = pinkBoxDrop.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
        rb.useGravity = true;

        return pinkBoxDrop;
    }

    private void dropBox()
    {
        removePinkCube();
    }

    private void pickupBox()
    {
        GameObject pinkBox = pinkBoxCollided.Pop(); pinkBoxCollided.Push(pinkBox);

        pinkBoxStackPickedUp.Push(pinkBox);
        pinkBox.GetComponent<SheepMovement>().setState((int)State.Unavailable);
        changePlayerSpeed();

        Rigidbody rb = pinkBox.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (0 < colligingWithBoxSem && pinkBoxStackPickedUp.ToArray().Length < 3)
            {
                pickupBox();
            }
            else
                if (pinkBoxStackPickedUp.ToArray().Length > 0)
                {
                   dropBox();
                }
        }

        updatePinkBoxRotations();
    }

    public void updatePinkBoxRotations()
    {
        int index = 0;
        GameObject[] arrPinkBoxes = pinkBoxStackPickedUp.ToArray();
        foreach (GameObject pB in arrPinkBoxes)
        {
            Vector3 newV = gameObject.transform.position;
            Vector3 oldNewV = newV;
            Quaternion newR = gameObject.transform.rotation;
            newV.y += 1;
            switch (index++)
            {
                case 0:
                    newV.y += 1.27f;
                    break;
                case 1:
                    newV.x += 1f;
                    break;
                case 2:
                    newV.x -= 1f;
                    break;
            }
            pB.transform.position = newV;
            pB.transform.rotation = newR;
            pB.transform.RotateAround(newV, Vector3.up, -newR.eulerAngles.y);
            pB.transform.RotateAround(oldNewV, Vector3.up, newR.eulerAngles.y);
        }
    }
    
}
