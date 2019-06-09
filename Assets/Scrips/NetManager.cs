using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetManager : MonoBehaviour
{

    public GameObject manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("NetworkManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (manager == null)
            manager = GameObject.FindGameObjectWithTag("NetworkManager");
    }

    public void startHost()
    {

        manager.GetComponent<NetworkManager>().StartHost();

    }

    public void startClient()
    {
        manager.GetComponent<NetworkManager>().StartClient();
    }

}
