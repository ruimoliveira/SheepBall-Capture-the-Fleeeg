using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkServerRelay : NetworkMessageHandler
{
    private void Start()
    {
        if(isServer)
        {
            RegisterNetworkMessages();
        }
    }

    private void RegisterNetworkMessages()
    {
        NetworkServer.RegisterHandler(player_movement_msg, OnReceivePlayerMovementMessage);
    }

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
    {
        PlayerMovementMessage _msg = _message.ReadMessage<PlayerMovementMessage>();
        NetworkServer.SendToAll(player_movement_msg, _msg);
    }

    //TODO check if this is legal
    public void SendSheepMovement(NetworkMessage _message)
    {
        SheepMovementMessage _msg = _message.ReadMessage<SheepMovementMessage>();
        NetworkServer.SendToAll(sheep_movement_msg, _msg);
    }

    //TODO check if this is legal
    public void SendmatchInfo(NetworkMessage _message)
    {
        MatchInfoMessage _msg = _message.ReadMessage<MatchInfoMessage>();
        NetworkServer.SendToAll(match_info_msg, _msg);
    }
}
