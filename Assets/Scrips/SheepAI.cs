using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using PlayerManager;

public class SheepAI : NetworkMessageHandler
{
    private List<GameObject> players = new List<GameObject>();
    private List<GameObject> sheeps = new List<GameObject>();
    private List<GameObject> available_sheeps = new List<GameObject>();

    private float timer = TIME_BETWEEN_MOVEMENT;

    enum State { Available, Rotating, Moving, Waiting, Unavailable, Scared };
    private const float TIME_BETWEEN_MOVEMENT = 1f;
    private const float SCARE_DISTANCE = 4f;

    // Update is called once per frame
    void FixedUpdate()
    {
        //SheepAI only runs in Server
        if (!isServer)
            return;

        timer -= Time.deltaTime;

        updatePlayers();
        scareSheep();

        if (timer <= 0)
        {
            updateAvailableSheeps();
            sheepMovement();
            resetTimer();
        }
    }

    public void updatePlayers()
    {
        players.Clear();
        foreach (GameObject p in Manager.Instance.GetConnectedPlayers())
        {
            players.Add(p.transform.Find("Graphics").gameObject);
        }
    }

    public void addSheep(GameObject sheep)
    {
        sheeps.Add(sheep);
    }

    void updateAvailableSheeps()
    {
        available_sheeps.Clear();
        foreach (GameObject sheep in sheeps)
        {
            if (sheep.GetComponent<SheepMovement>().getState() == (int)State.Available)
            {
                available_sheeps.Add(sheep);
            }
        }
    }

    void scareSheep()
    {
        foreach (GameObject player in players)
        {
            playerScareSheep(player, sheeps);
        }
    }

    void playerScareSheep(GameObject player, List<GameObject> sheeps)
    {
        foreach (GameObject sheep in sheeps)
        {
            float distance = Vector3.Distance(player.transform.position, sheep.transform.position);
            var sheep_script = sheep.GetComponent<SheepMovement>();

            if (sheep_script.getState() != (int)State.Unavailable && distance < SCARE_DISTANCE)
            {
                var opposite_direction = sheep.transform.position - player.transform.position;
                sheep_script.scare(opposite_direction);
            }
            else if(sheep_script.getState() == (int)State.Scared && distance >= SCARE_DISTANCE)
            {
                sheep_script.unscare();
            }
        }
    }

    void sheepMovement()
    {
        int numSheepsToMove = getNumSheepsToMove(available_sheeps.Count);  //get the number of sheep to move based on ammount of available sheeps

        if (numSheepsToMove == -1)
            return;

        List<int> randomSheepIndexes = calculateRandomIndexes(available_sheeps, numSheepsToMove);
        GameObject[] sheepsToMove = getSheepsToMove(available_sheeps, randomSheepIndexes);

        moveSheeps(sheepsToMove);
    }

    void resetTimer()
    {
        timer = TIME_BETWEEN_MOVEMENT;
    }

    int getNumSheepsToMove(int totalSheeps)
    {
        if (totalSheeps < 2)
            return -1;
        return 2; //TO DO (% of total Sheeps + random factor)
    }

    List<int> calculateRandomIndexes(List<GameObject> available_sheeps, int numSheepsToMove)
    {
        List<int> randomIndexes = new List<int>();
        System.Random rnd = new System.Random();

        for (int i = 0; i < numSheepsToMove; i++)
        {
            int number;
            do
            {
                number = rnd.Next(0, available_sheeps.Count);
            }
            while (randomIndexes.Contains(number));

            randomIndexes.Add(number);
        }

        return randomIndexes;
    }

    GameObject[] getSheepsToMove(List<GameObject> available_sheeps, List<int> randomSheepIndexes)
    {
        GameObject[] sheepsToMove = new GameObject[randomSheepIndexes.Count];
        for (int i = 0; i < randomSheepIndexes.Count; i++)
            sheepsToMove[i] = available_sheeps[randomSheepIndexes[i]];

        return sheepsToMove;
    }

    void moveSheeps(GameObject[] sheepsToMove)
    {
        foreach (GameObject sheep in sheepsToMove)
        {
            var sheep_script = sheep.GetComponent<SheepMovement>();
            StartCoroutine(sheep_script.move());
        }
    }

    public void processSheepMovementMessage(SheepMovementMessage _msg)
    {
        //server does not care about sheep movement receiving messages
        if(isServer)
            return;

        GameObject sheep = GameObject.Find(_msg.objectTransformName);

        if (sheep != null)
        {
            SheepMovement sheep_movement = sheep.GetComponent<SheepMovement>();

            if (sheep_movement.getState() != (int)State.Unavailable)
            {
                sheep_movement.localMove(_msg.objectPosition, _msg.objectRotation, _msg.time, _msg.objectAnimation);
            }
        }
    }

    public void processPickedUpSheepMessage(PickedUpSheepMessage _msg)
    {
        GameObject sheep = GameObject.Find(_msg.sheepName);
       
        if (sheep != null)
        {
            SheepMovement sheep_movement = sheep.GetComponent<SheepMovement>();
            Rigidbody sheep_rb = sheep.GetComponent<Rigidbody>();

            sheep_rb.velocity = Vector3.zero;
            sheep_rb.angularVelocity = Vector3.zero;
            sheep_rb.useGravity = false;

            sheep_movement.setPickedUpBy(_msg.playerName);
            sheep_movement.setState(_msg.sheepState);
            sheep_movement.localMove(_msg.sheepPosition, _msg.sheepRotation, _msg.time, _msg.sheepAnimation);
        }
    }
    public void processDroppedSheepMessage(DroppedSheepMessage _msg)
    {
        GameObject sheep = GameObject.Find(_msg.sheepName);

        if (sheep != null)
        {
            SheepMovement sheep_movement = sheep.GetComponent<SheepMovement>();
            Rigidbody sheep_rb = sheep.GetComponent<Rigidbody>();

            sheep_rb.velocity = Vector3.zero;
            sheep_rb.useGravity = true;

            sheep_movement.setPickedUpBy("");
            sheep_movement.setState(_msg.sheepState);
        }
    }

    /*bool hasValue(int[] array, int value)
    {
        if (array.Length == 0)
            return false;

        for(int i = 0; i < array.Length; i++)
        {
            if (array[i] == value)
                return true;
        }
        return false;
    }*/
}
