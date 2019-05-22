using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class SheepAI : MonoBehaviour
{
    private GameObject[] players;
    private List<GameObject> sheeps = new List<GameObject>();
    private List<GameObject> available_sheeps = new List<GameObject>();

    private float timer = TIME_BETWEEN_MOVEMENT;

    enum State { Available, Rotating, Moving, Waiting, Unavailable, Scared };
    private const float TIME_BETWEEN_MOVEMENT = 1f;
    private const float SCARE_DISTANCE = 4f;

    // Start is called before the first frame update
    void Start()
    {
        return;
        players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log("players: " + players.Length);

        updateSheeps();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        return;
        timer -= Time.deltaTime;

        // TO DO: overwrite the 'sheeps' variable every frame OR the spawn script overwrites this 'sheeps' variable when a change happens

        scareSheep();

        if (timer <= 0)
        {
            updateAvailableSheeps();
            sheepMovement();
            resetTimer();
        }
    }

    public void updateSheeps()
    {
        sheeps.Clear();
        foreach (Transform child in transform)
        {
            var sheep = child.gameObject;
            sheeps.Add(sheep);
        }
    }

    void updateAvailableSheeps()
    {
        available_sheeps.Clear();
        foreach (Transform child in transform)
        {
            var sheep = child.gameObject;

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
