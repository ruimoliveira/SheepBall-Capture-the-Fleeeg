using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using PlayerManager;

public class UIStateManager : NetworkMessageHandler
{
    public const string DEFAULT_NAME = "Empty Slot";

    public GameObject networkManagerObj;
    public GameObject gameStateManagerObj;
    public GameObject canvasObj;

    public GameObject[] teamRedPlayerNames = new GameObject[2];
    public GameObject[] teamBluePlayerNames = new GameObject[2];
    public GameObject[] teamYellowPlayerNames = new GameObject[2];

    // Start is called before the first frame update
    void Start()
    {
        gameStateManagerObj = GameObject.FindGameObjectWithTag("GameStateManager");
        networkManagerObj = GameObject.FindGameObjectWithTag("NetworkManager");
    }

    // Update is called once per frame
    void Update()
    {
        if (networkManagerObj == null)
            networkManagerObj = GameObject.FindGameObjectWithTag("NetworkManager");

        if (gameStateManagerObj != null)
            updateUI();
        else
            gameStateManagerObj = GameObject.FindGameObjectWithTag("GameStateManager");
    }

    public void leaveLobbyBtn()
    {
        bool isHost = Manager.Instance.ConnectedPlayers[Manager.Instance.PlayerID].GetComponent<NetworkIdentity>().isServer;

        //remove all conections from player manager
        for(int i = 0; i < Manager.Instance.ConnectedPlayers.Count; i++)
        {
            Manager.Instance.RemovePlayerFromConnectedPlayers(Manager.Instance.GetConnectedPlayers()[i].transform.name);
        }
        

        if (isHost)
            networkManagerObj.GetComponent<NetworkManager>().StopHost();      
        else
            networkManagerObj.GetComponent<NetworkManager>().StopClient();

        //reset network manager
        NetworkManager.networkSceneName = "";
    }

    public void joinTeamRedBtn()
    {
        JoinTeamMessage _msg = new JoinTeamMessage()
        {
            objectTransformName = Manager.Instance.PlayerID,
            nickname = Manager.Instance.PlayerID,
            team = 1
        };

        NetworkManager.singleton.client.Send(join_team_msg, _msg);
    }

    public void joinTeamBlueBtn()
    {
        JoinTeamMessage _msg = new JoinTeamMessage()
        {
            objectTransformName = Manager.Instance.PlayerID,
            nickname = Manager.Instance.PlayerID,
            team = 2
        };

        NetworkManager.singleton.client.Send(join_team_msg, _msg);
    }

    public void joinTeamYellowBtn()
    {
        JoinTeamMessage _msg = new JoinTeamMessage()
        {
            objectTransformName = Manager.Instance.PlayerID,
            nickname = Manager.Instance.PlayerID,
            team = 3
        };

        NetworkManager.singleton.client.Send(join_team_msg, _msg);
    }

    public void updateUI()
    {
        if (gameStateManagerObj == null)
            return;

        //player 0
        if (gameStateManagerObj.GetComponent<GameStateManager>().teamRedPlayers[0] != null)
            teamRedPlayerNames[0].GetComponent<Text>().text = gameStateManagerObj.GetComponent<GameStateManager>().teamRedPlayers[0].transform.name;
        else
            teamRedPlayerNames[0].GetComponent<Text>().text = DEFAULT_NAME;

        //player 1
        if (gameStateManagerObj.GetComponent<GameStateManager>().teamRedPlayers[1] != null)
            teamRedPlayerNames[1].GetComponent<Text>().text = gameStateManagerObj.GetComponent<GameStateManager>().teamRedPlayers[1].transform.name;
        else
            teamRedPlayerNames[1].GetComponent<Text>().text = DEFAULT_NAME;

        //player 2
        if (gameStateManagerObj.GetComponent<GameStateManager>().teamBluePlayers[0] != null)
            teamBluePlayerNames[0].GetComponent<Text>().text = gameStateManagerObj.GetComponent<GameStateManager>().teamBluePlayers[0].transform.name;
        else
            teamBluePlayerNames[0].GetComponent<Text>().text = DEFAULT_NAME;

        //player 3
        if (gameStateManagerObj.GetComponent<GameStateManager>().teamBluePlayers[1] != null)
            teamBluePlayerNames[1].GetComponent<Text>().text = gameStateManagerObj.GetComponent<GameStateManager>().teamBluePlayers[1].transform.name;
        else
            teamBluePlayerNames[1].GetComponent<Text>().text = DEFAULT_NAME;

        //player 4
        if (gameStateManagerObj.GetComponent<GameStateManager>().teamYellowPlayers[0] != null)
            teamYellowPlayerNames[0].GetComponent<Text>().text = gameStateManagerObj.GetComponent<GameStateManager>().teamYellowPlayers[0].transform.name;
        else
            teamYellowPlayerNames[0].GetComponent<Text>().text = DEFAULT_NAME;

        //player 5
        if (gameStateManagerObj.GetComponent<GameStateManager>().teamYellowPlayers[1] != null)
            teamYellowPlayerNames[1].GetComponent<Text>().text = gameStateManagerObj.GetComponent<GameStateManager>().teamYellowPlayers[1].transform.name;
        else
            teamYellowPlayerNames[1].GetComponent<Text>().text = DEFAULT_NAME;
    }

}
