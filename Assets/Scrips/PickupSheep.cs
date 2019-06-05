using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;
using UnityEngine.Networking;
using SheepAnimationState;

public class PickupSheep : NetworkMessageHandler
{
    List<GameObject> sheepColliding;
    Stack<GameObject> sheepPickedup;
    ThirdPersonUserControl userControls;
    GameObject[] baseWalls;

    [Header("PickedUp Sheep Movement Properties")]
    public bool canSendNetworkMovement;
    public float speed;
    public float networkSendRate = 5;
    public float timeBetweenMovementStart;
    public float timeBetweenMovementEnd;

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

        if (!canSendNetworkMovement)
        {
            canSendNetworkMovement = true;
            StartCoroutine(StartNetworkSendCooldown());
        }
    }

    private IEnumerator StartNetworkSendCooldown()
    {
        timeBetweenMovementStart = Time.time;
        yield return new WaitForSeconds((1 / networkSendRate));
        SendNetworkMovement();
    }

    private void SendNetworkMovement()
    {
        timeBetweenMovementEnd = Time.time;

        GameObject[] carrying = sheepPickedup.ToArray();
        foreach(GameObject sheep in carrying){
            int state = sheep.GetComponent<SheepMovement>().getState();
            int anim_index = sheep.GetComponentInChildren<Animator>().GetInteger("Index");
            SendPickedUpSheepMessage(sheep.transform.name, sheep.transform.position, sheep.transform.rotation, (timeBetweenMovementEnd - timeBetweenMovementStart), state, anim_index);
        }

        canSendNetworkMovement = false;
    }

    public void SendPickedUpSheepMessage(string _sheepID, Vector3 _position, Quaternion _rotation, float _timeTolerp, int state, int anim_index)
    {
        PickedUpSheepMessage _msg = new PickedUpSheepMessage()
        {
            playerName = transform.name,
            sheepName = _sheepID,
            sheepPosition = _position,
            sheepRotation = _rotation,
            time = _timeTolerp,
            sheepState = state,
            sheepAnimation = anim_index
        };

        //NetworkServer.SendToAll(sheep_movement_msg, _msg);
        NetworkManager.singleton.client.Send(picked_up_sheep_message, _msg);
    }

    public void SendDroppedSheepMessage(string _sheepID, int state, int anim_index)
    {
        DroppedSheepMessage _msg = new DroppedSheepMessage()
        {
            playerName = transform.name,
            sheepName = _sheepID,
            sheepState = state,
            sheepAnimation = anim_index
        };

        //NetworkServer.SendToAll(sheep_movement_msg, _msg);
        NetworkManager.singleton.client.Send(dropped_sheep_message, _msg);
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

        SheepMovement droppedSheep_movement = droppedSheep.GetComponentInChildren<SheepMovement>();
        Animator droppedSheep_animator = droppedSheep.GetComponentInChildren<Animator>();
        Rigidbody droppedSheep_rb = droppedSheep.GetComponent<Rigidbody>();

        // set state to available and anim state to iddle
        droppedSheep_movement.setAvailable();
        IAnimState animState = new Iddle(ref droppedSheep_animator);
        droppedSheep_movement.SetAnimState(animState);

        sheepCollideWithBases(droppedSheep);
        changePlayerSpeed();

        droppedSheep_rb.velocity = Vector3.zero;
        droppedSheep_rb.useGravity = true;

        SendDroppedSheepMessage(droppedSheep.transform.name, droppedSheep_movement.getState(), droppedSheep_animator.GetInteger("Index"));

        return droppedSheep;
    }

    private void pickupSheep()
    {
        GameObject sheepToPickup = sheepColliding[0];
        sheepColliding.RemoveAt(0);

        SheepMovement sheepToPickup_movement = sheepToPickup.GetComponentInChildren<SheepMovement>();
        Animator sheepToPickup_animator = sheepToPickup.GetComponentInChildren<Animator>();
        Rigidbody sheepToPickup_rb = sheepToPickup.GetComponent<Rigidbody>();

        // set state to unavaiable and anim state to ball
        sheepToPickup_movement.setUnavailable();
        IAnimState animState = new Ball(ref sheepToPickup_animator);
        sheepToPickup_movement.SetAnimState(animState);
        
        sheepGhostBases(sheepToPickup);

        sheepToPickup_rb.velocity = Vector3.zero;
        sheepToPickup_rb.angularVelocity = Vector3.zero;
        sheepToPickup_rb.useGravity = false;

        sheepPickedup.Push(sheepToPickup);
        changePlayerSpeed();
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

            sheep.transform.Rotate(new Vector3(0, 180, 0),Space.Self);
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
