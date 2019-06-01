using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
{
    public GameObject playerUnitPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        CmdSpawnMyUnit();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Command]
    void CmdSpawnMyUnit()
    {
        GameObject unit = Instantiate(playerUnitPrefab);
        NetworkServer.SpawnWithClientAuthority(unit, connectionToClient);
    }
}
