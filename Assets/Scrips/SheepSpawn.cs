using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class SheepSpawn : NetworkBehaviour
{
    private Vector2 SPAWN_AREA_X_RANGE;
    private Vector2 SPAWN_AREA_Z_RANGE;

    private float spawnTimer = 0.0f;
    private bool spawnReady = true;

    private GameObject sheepPrefab = null;
    public GameObject sheepCollection;
    private GameObject[] players;

    // Start is called before the first frame update
    void Awake()
    {
        this.sheepPrefab = (GameObject) Resources.Load<GameObject>("Prefabs/Sheep");
        Debug.Log(this.sheepPrefab != null);
        // this.sheepCollection = GameObject.Find("/SheepManager");
        this.players = GameObject.FindGameObjectsWithTag("Player");

        CalculateSpawnLimits();

        // Spawn initial sheep
        /*
        for (int i = 1; i <= Constants.MAX_NEUTRAL_SHEEP; i++)
        {
            SpawnNewSheep();
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        //only server can spawn sheep
        if (!isServer)
            return;

        if (!this.spawnReady)
        {
            DecreaseTimer(Time.deltaTime);
            return;
        }

        if (SheepAmountBelowThreshold() && this.spawnReady)
            SpawnNewSheep();
    }

    void SpawnNewSheep()
    {
        float randomX = Random.Range(SPAWN_AREA_X_RANGE[0], SPAWN_AREA_X_RANGE[1]);
        float randomZ = Random.Range(SPAWN_AREA_Z_RANGE[0], SPAWN_AREA_Z_RANGE[1]);
        Vector3 spawnPosition = new Vector3(randomX, 0.1f, randomZ);
        GameObject newSheep = Instantiate(this.sheepPrefab, spawnPosition, Quaternion.identity);
        
        newSheep.transform.SetParent(this.sheepCollection.transform);
        Debug.Log("SHEEP COLLECTION: " + this.sheepCollection);
        newSheep.name = "Sheep" + this.sheepCollection.transform.childCount;

        foreach (GameObject player in this.players)
        {
            Physics.IgnoreCollision(newSheep.GetComponent<Collider>(), player.GetComponent<Collider>()); // Ignore collision with players
        }

        NetworkServer.Spawn(newSheep);
        this.sheepCollection.GetComponent<SheepAI>().updateSheeps();
        
        StartTimer();
    }

    void DecreaseTimer(float elapsedTime)
    {
        this.spawnTimer -= elapsedTime;
        this.spawnReady = this.spawnTimer <= 0;
    }

    void StartTimer()
    {
        this.spawnTimer = Constants.SPAWN_COOLDOWN;
        this.spawnReady = false;
    }

    bool SheepAmountBelowThreshold()
    {
        return GameObject.FindGameObjectsWithTag(Constants.SHEEP_TAG).Length < Constants.MAX_NEUTRAL_SHEEP;
    }

    void CalculateSpawnLimits()
    {
        Vector3 spawnBaseSize = GetComponent<MeshFilter>().mesh.bounds.size;
        Vector3 spawnScale = transform.localScale;
        Vector3 spawnHalfSize = new Vector3((spawnBaseSize.x * spawnScale.x) / 2, 0, (spawnBaseSize.z * spawnScale.z) / 2);

        Vector3 spawnAreaPosition = transform.position;

        this.SPAWN_AREA_X_RANGE = new Vector2(spawnAreaPosition.x - spawnHalfSize.x, spawnAreaPosition.x + spawnHalfSize.x);
        this.SPAWN_AREA_Z_RANGE = new Vector2(spawnAreaPosition.z - spawnHalfSize.z, spawnAreaPosition.z + spawnHalfSize.z);

        Debug.Log("SPAWN BASE SIZE" + spawnBaseSize.ToString());
        Debug.Log("SPAWN SCALE" + spawnScale.ToString());
        Debug.Log("SPAWN REAL SIZE" + spawnHalfSize.ToString());
        Debug.Log("SPAWN CENTER" + spawnAreaPosition);
        Debug.Log("SPAWN X RANGE" + SPAWN_AREA_X_RANGE[0]);
        Debug.Log("SPAWN Z RANGE" + SPAWN_AREA_Z_RANGE[1]);
    }
}
