using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkServerRelay : NetworkMessageHandler
{
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (isServer)
        {
            RegisterNetworkMessages();
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkServer.RegisterHandler(lobby_info_msg, OnReceiveLobbyInfoMessage);
        NetworkServer.RegisterHandler(join_team_msg, OnReceiveJoinTeamMessage);
        NetworkServer.RegisterHandler(match_info_msg, SendmatchInfo);

		NetworkServer.RegisterHandler(player_movement_msg, SendPlayerMovement);
        NetworkServer.RegisterHandler(sheep_movement_msg, SendSheepMovement);
        NetworkServer.RegisterHandler(picked_up_sheep_message, SendPickedUpSheep);
        NetworkServer.RegisterHandler(dropped_sheep_message, SendDroppedSheep);
        NetworkServer.RegisterHandler(shoot_sheep_message, SendShootSheep);
        NetworkServer.RegisterHandler(land_sheep_message, SendLandSheep);
    }

    private void OnReceiveLobbyInfoMessage(NetworkMessage _message)
    {
        LobbyInfoMessage _msg = _message.ReadMessage<LobbyInfoMessage>();
        NetworkServer.SendToAll(lobby_info_msg, _msg);
    }

    //é necessário?
    private void OnReceiveJoinTeamMessage(NetworkMessage _message)
    {
        JoinTeamMessage _msg = _message.ReadMessage<JoinTeamMessage>();
        NetworkServer.SendToAll(join_team_msg, _msg);
    }

    private void SendPlayerMovement(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();
        NetworkServer.SendToAll(player_movement_msg, _msg);
    }

    public void SendSheepMovement(NetworkMessage _message)
    {
        SheepMovementMessage _msg = _message.ReadMessage<SheepMovementMessage>();
        NetworkServer.SendToAll(sheep_movement_msg, _msg);
    }

    public void SendmatchInfo(NetworkMessage _message)
	{

	}

    public void SendPickedUpSheep(NetworkMessage _message)
    {
        PickedUpSheepMessage _msg = _message.ReadMessage<PickedUpSheepMessage>();
        NetworkServer.SendToAll(picked_up_sheep_message, _msg);
    }

    public void SendDroppedSheep(NetworkMessage _message)
    {
        DroppedSheepMessage _msg = _message.ReadMessage<DroppedSheepMessage>();
        NetworkServer.SendToAll(dropped_sheep_message, _msg);
    }

    public void SendShootSheep(NetworkMessage _message)
    {
        ShootSheepMessage _msg = _message.ReadMessage<ShootSheepMessage>();

        GameObject sheepManager = GameObject.Find("SheepManager");
        GameObject sheep = sheepManager.transform.Find(_msg.sheepName).gameObject;
        Rigidbody sheep_rb = sheep.GetComponent<Rigidbody>();

        sheep_rb.AddRelativeForce(new Vector3(0, 0.1f, -1.2f) * _msg.impulse, ForceMode.Impulse);

        NetworkServer.SendToAll(shoot_sheep_message, _msg);
    }

    public void SendLandSheep(NetworkMessage _message)
    {
        LandSheepMessage _msg = _message.ReadMessage<LandSheepMessage>();
        NetworkServer.SendToAll(land_sheep_message, _msg);
    }
}
