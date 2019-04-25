using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using UnityEngine;

public class SheepAI : MonoBehaviour
{ 
    private List<GameObject> sheeps = new List<GameObject>();
    private List<GameObject> available_sheeps = new List<GameObject>();

    private float timer = TIME_BETWEEN_MOVEMENT;

    enum State { Available, Moving, Unavailable };
    private const float TIME_BETWEEN_MOVEMENT = 1f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            updateSheeps();
            moveSheeps();
            resetTimer();
        }
    }   

    void updateSheeps()
    {
        sheeps.Clear();
        available_sheeps.Clear();
        foreach (Transform child in transform)
        {
            var sheep = child.gameObject;
            sheeps.Add(sheep);

            if (sheep.GetComponent<SheepMovement>().getState() == (int) State.Available)
            {
                available_sheeps.Add(sheep);
            }
        }
    }

    void moveSheeps()
    {
        int numSheepsToMove = getNumSheepsToMove(available_sheeps.Count);  //get the number of sheep to move based on ammount of available sheeps

        if (numSheepsToMove == -1)
        {
            Debug.Log("No available sheep to move");
            return;
        }
            
        List<int> randomSheepIndexes = calculateRandomIndexes(available_sheeps, numSheepsToMove);

        GameObject[] sheepsToMove = new GameObject[numSheepsToMove];
        for(int i = 0; i < numSheepsToMove; i++)
            sheepsToMove[i] = available_sheeps[randomSheepIndexes[i]];

        foreach (GameObject sheep in sheepsToMove)
        {
            var sheep_script = sheep.GetComponent<SheepMovement>();

            StartCoroutine(sheep_script.move());
        }
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

        for(int i = 0; i < numSheepsToMove; i++)
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
