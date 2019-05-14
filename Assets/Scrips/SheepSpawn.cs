using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SheepSpawn : MonoBehaviour
{
    const string NEUTRAL_SHEEP_TAG = "PinkBox"; // CHANGE TAG WHEN OTHER SCRIPTS ADMIT "NeutralSheep" TAG
    const string SPAWN_AREA_NAME = "SpawnArea";
    const int MAX_NEUTRAL_SHEEP = 10;
    const float SPAWN_COOLDOWN = 3.0f; // seconds

    private Vector2 SPAWN_AREA_X_RANGE;
    private Vector2 SPAWN_AREA_Z_RANGE;
    private float spawnTimer = 0.0f;
    private bool spawnReady = true;

    private GameObject sheepPrefab;
    private GameObject sheepCollection;
    private GameObject[] players;

    // Start is called before the first frame update
    void Start()
    {   
        this.sheepPrefab = (GameObject) AssetDatabase.LoadAssetAtPath("Assets/Prefabs/Sheep.prefab", typeof(GameObject));
        this.sheepCollection = GameObject.Find("/Sheep");
        this.players = GameObject.FindGameObjectsWithTag("Player");

        // Calculte sheep spawn area limits
        GameObject spawnArea = GameObject.Find(SPAWN_AREA_NAME);

        Vector3 spawnBaseSize = spawnArea.GetComponent<MeshFilter>().mesh.bounds.size;
        Vector3 spawnScale = spawnArea.transform.localScale;
        Vector3 spawnHalfSize = new Vector3((spawnBaseSize.x * spawnScale.x)/2, 0, (spawnBaseSize.z * spawnScale.z)/2);

        Vector3 spawnAreaPosition = spawnArea.transform.position;

        this.SPAWN_AREA_X_RANGE = new Vector2(spawnAreaPosition.x - spawnHalfSize.x, spawnAreaPosition.x + spawnHalfSize.x);
        this.SPAWN_AREA_Z_RANGE = new Vector2(spawnAreaPosition.z - spawnHalfSize.z, spawnAreaPosition.z + spawnHalfSize.z);

        Debug.Log("SPAWN BASE SIZE" + spawnBaseSize.ToString());
        Debug.Log("SPAWN SCALE" + spawnScale.ToString());
        Debug.Log("SPAWN REAL SIZE" + spawnHalfSize.ToString());
        Debug.Log("SPAWN CENTER" + spawnAreaPosition);
        Debug.Log("SPAWN X RANGE" + SPAWN_AREA_X_RANGE[0]);
        Debug.Log("SPAWN Z RANGE" + SPAWN_AREA_Z_RANGE[1]);

        // Spawn initial sheep
        for(int i = 1; i <= MAX_NEUTRAL_SHEEP; i++)
        {
            SpawnNewSheep();
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        newSheep.name = "Sheep" + this.sheepCollection.transform.childCount;
        newSheep.tag = NEUTRAL_SHEEP_TAG;

        foreach (GameObject player in this.players)
        {
            Physics.IgnoreCollision(newSheep.GetComponent<Collider>(), player.GetComponent<Collider>()); // Ignore collision with players
        }

        this.sheepCollection.GetComponent<SheepAI>().updateSheeps();
        Debug.Log("SHEEP SPAWNED AT POS" + spawnPosition.ToString());

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
