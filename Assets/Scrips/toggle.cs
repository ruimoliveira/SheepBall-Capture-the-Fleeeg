using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toggle : MonoBehaviour
{
    public void OnChangeValue()
    {
        GameObject.Find("/SpawnArea").GetComponent<SheepSpawn>().enabled = true;
        GameObject.Find("/SheepManager").GetComponent<SheepAI>().enabled = true;
    }
}
