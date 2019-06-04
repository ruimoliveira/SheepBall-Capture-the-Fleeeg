using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class PickupSheep : MonoBehaviour
{
    List<GameObject> sheepColliding;
    Stack<GameObject> sheepPickedup;
    ThirdPersonUserControl userControls;
    GameObject[] baseWalls;

    public Stack<GameObject> getSheepStack()
    {
        return sheepPickedup;
    }

    private void Awake()
    {
        userControls = GetComponent<ThirdPersonUserControl>();
        sheepPickedup = new Stack<GameObject>();
        sheepColliding = new List<GameObject>();
        baseWalls = GameObject.FindGameObjectsWithTag(Constants.BASE_WALL_TAG);

        // Ignore collision between player and sheep
        GameObject[] allSheep = GameObject.FindGameObjectsWithTag(Constants.SHEEP_TAG);
        foreach (GameObject sheep in allSheep)
        {
            Physics.IgnoreCollision(sheep.GetComponent<Collider>(), GetComponent<Collider>());
        }
    }

    private void OnTriggerEnter(Collider body)
    {
        if (body.tag == Constants.SHEEP_TAG)
        {
            var sheep = body.gameObject;
            sheepColliding.Add(sheep);
        }
    }

    private void OnTriggerExit(Collider body)
    {
        if (body.tag == Constants.SHEEP_TAG)
        {
            for (var i = 0; i < sheepColliding.Count; i++)
            {
                if(GameObject.ReferenceEquals(body.gameObject, sheepColliding[i]))
                {
                    sheepColliding.RemoveAt(i);
                }
            }
        }
    }

    private void changePlayerSpeed()
    {
        switch (sheepPickedup.Count)
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

    public GameObject dropSheep()
    {
        if (sheepPickedup.Count == 0)
            return null;

        GameObject droppedSheep = sheepPickedup.Pop();
        //droppedSheep.GetComponent<SheepMovement>().setAvailable();
        droppedSheep.GetComponent<SheepMovement>().setFlying();
        sheepCollideWithBases(droppedSheep);
        changePlayerSpeed();

        Rigidbody rb = droppedSheep.GetComponent<Rigidbody>();
        rb.velocity = new Vector3(0, 0, 0);
        rb.useGravity = true;

        return droppedSheep;
    }

    private void pickupSheep()
    {
        GameObject sheepToPickup = sheepColliding[0];
        sheepColliding.RemoveAt(0);
       
        sheepToPickup.GetComponent<SheepMovement>().setUnavailable();
        sheepGhostBases(sheepToPickup);
        
        Rigidbody rb = sheepToPickup.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;

        sheepPickedup.Push(sheepToPickup);
        changePlayerSpeed();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (sheepColliding.Count > 0 && sheepPickedup.Count < Constants.MAX_SHEEP_CARRIED)
            {
                pickupSheep();
            }
            else
            {
                dropSheep();
            }
        }

        updateSheepRotation();
    }

    public void updateSheepRotation()
    {
        if (sheepPickedup.Count == 0) return;
        int index = 0;
        GameObject[] arrPinkBoxes = sheepPickedup.ToArray();
        foreach (GameObject sheep in sheepPickedup)
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
            sheep.transform.position = newV;
            sheep.transform.rotation = newR;
            sheep.transform.RotateAround(newV, Vector3.up, -newR.eulerAngles.y);
            sheep.transform.RotateAround(oldNewV, Vector3.up, newR.eulerAngles.y);
        }
    }
    
    // Avoid sheep from colliding with base walls
    private void sheepGhostBases(GameObject sheep)
    {
        foreach (GameObject wall in baseWalls)
        {
            Physics.IgnoreCollision(sheep.GetComponent<Collider>(), wall.GetComponent<Collider>());
        }
    }

    // Allow sheep to collide with base walls
    private void sheepCollideWithBases(GameObject sheep)
    {
        foreach (GameObject wall in baseWalls)
        {
            Physics.IgnoreCollision(sheep.GetComponent<Collider>(), wall.GetComponent<Collider>(), false);
        }
    }
}
