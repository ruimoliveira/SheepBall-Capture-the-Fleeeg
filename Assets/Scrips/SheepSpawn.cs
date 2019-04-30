using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpawn : MonoBehaviour
{
    const string NEUTRAL_SHEEP_TAG = "PinkBox"; // MUDAR PARA 'neutral'
    const int MAX_NEUTRAL_SHEEP = 5;
    const float SPAWN_COOLDOWN = 15.0f; // seconds

    private float spawnTimer = 0.0f;
    private bool spawnReady = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!spawnReady)
            DecreaseTimer(Time.deltaTime);
        //TODO: START TIMER WHEN SHEEP GETS CAPTURED
        if (SheepAmountBelowThreshold() && this.spawnReady)
            SpawnNewSheep();
    }

    void SpawnNewSheep()
    {
        // TODO: SPAWN SHEEP
        StartTimer();
    }

    void DecreaseTimer(float elapsedTime)
    {
        this.spawnTimer -= elapsedTime;
        this.spawnReady = this.spawnTimer <= 0;
    }

    void StartTimer()
    {
        this.spawnTimer = SPAWN_COOLDOWN;
        this.spawnReady = false;
    }

    bool SheepAmountBelowThreshold()
    {
        return GameObject.FindGameObjectsWithTag(NEUTRAL_SHEEP_TAG).Length < MAX_NEUTRAL_SHEEP;
    }
}
