using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : MonoBehaviour
{

    public NetworkManager manager;
    //public bool done = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (done)
        //    return;

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            manager.StartHost();
            //manager.StartClient();
            //    done = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            manager.StartClient();
        //    done = true;
        }
    }
}
