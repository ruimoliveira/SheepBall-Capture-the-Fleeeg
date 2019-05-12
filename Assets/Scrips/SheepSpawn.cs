using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SheepSpawn : MonoBehaviour
{
    const string NEUTRAL_SHEEP_TAG = "PinkBox"; // MUDAR PARA 'neutral'
    const string SPAWN_AREA_NAME = "SpawnArea";
    const int MAX_NEUTRAL_SHEEP = 5;
    const float SPAWN_COOLDOWN = 15.0f; // seconds

    public GameObject sheepPrefab;

    private float spawnTimer = 0.0f;
    private bool spawnReady = true;

    // Start is called before the first frame update
    void Start()
    {
        GameObject spawnArea = GameObject.Find(SPAWN_AREA_NAME);
        Vector3 spawnBaseSize = spawnArea.GetComponent<MeshFilter>().mesh.bounds.size;
        Vector3 spawnScale = spawnArea.transform.localScale;
        Vector3 spawnSize = new Vector3(spawnBaseSize.x * spawnScale.x, 0, spawnBaseSize.z * spawnScale.z);

        Vector3 spawnCenter = spawnArea.transform.position;
        Debug.Log("SPAWN BASE SIZE" + spawnBaseSize.ToString());
        Debug.Log("SPAWN SCALE" + spawnScale.ToString());
        Debug.Log("SPAWN REAL SIZE" + spawnSize.ToString());
        Debug.Log("SPAWN CENTER" + spawnCenter.ToString());
        //Debug.Log(spawnLimits[0]);
    }

    // Update is called once per frame
    void Update()
    {
        if (!this.spawnReady)
        {
            DecreaseTimer(Time.deltaTime);
            return;
        }

        //TODO: START TIMER WHEN SHEEP GETS CAPTURED
        if (SheepAmountBelowThreshold() && this.spawnReady)
            SpawnNewSheep();
    }

    void SpawnNewSheep()
    {
        // TODO: SPAWN SHEEP
        Instantiate(this.sheepPrefab, new Vector3(0, 0, 0), Quaternion.identity);

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
