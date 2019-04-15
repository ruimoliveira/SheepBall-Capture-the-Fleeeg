using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoringPlane : MonoBehaviour
{
    string pinkBoxTag = "PinkBox";
    Text scoreText = null;
    int score = 0;

    private void Awake()
    {
        //scoreText = GameObject.FindGameObjectWithTag("Score").GetComponent<Text>();
    }

    private void changeScoreInUI()
    {
        //scoreText.text = "Score: " + score;
        Debug.Log("Score: " + score);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == pinkBoxTag)
        {
            score++;
            changeScoreInUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == pinkBoxTag)
        {
            score--;
            changeScoreInUI();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
