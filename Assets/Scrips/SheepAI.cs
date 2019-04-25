using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SheepAI : MonoBehaviour
{ 
    private List<GameObject> sheeps = new List<GameObject>();
    private float timer = TIME_BETWEEN_MOVEMENT;

    enum State { Ready, Busy };
    private const float TIME_BETWEEN_MOVEMENT = 2f;

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
        foreach (Transform child in transform)
        {
            sheeps.Add(child.gameObject);
        }
    }

    void moveSheeps()
    {

        int numSheepsToMove = getNumSheepsToMove(sheeps.Count);  //get the number of sheeps to move based on the total ammount of sheeps in play
        List<int> randomSheepIndexes = calculateRandomIndexes(numSheepsToMove);

        GameObject[] sheepsToMove = new GameObject[numSheepsToMove];
        for(int i = 0; i < numSheepsToMove; i++)
            sheepsToMove[i] = sheeps[randomSheepIndexes[i]];

        foreach (GameObject sheep in sheepsToMove)
        {
            var sheep_script = sheep.GetComponent<SheepMovement>();

            //TO DO: definir 3 variavieis direcao, distancia e tempo parado
            //passar ao move as 3 variaveis

            sheep_script.move();
        }
    }

    void resetTimer()
    {
        timer = TIME_BETWEEN_MOVEMENT;
    }

    int getNumSheepsToMove(int totalSheeps)
    {
        return 2; //TO DO (% of total Sheeps)
    }

    List<int> calculateRandomIndexes(int numSheepsToMove)
    {
        List<int> randomIndexes = new List<int>();
        System.Random rnd = new System.Random();

        for(int i = 0; i < numSheepsToMove; i++)
        {
            int number;
            do
            {
                number = rnd.Next(0, sheeps.Count);
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
