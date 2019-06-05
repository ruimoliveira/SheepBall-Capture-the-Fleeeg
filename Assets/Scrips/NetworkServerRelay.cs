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
        NetworkServer.RegisterHandler(player_movement_msg, SendPlayerMovement);
        NetworkServer.RegisterHandler(sheep_movement_msg, SendSheepMovement);
        NetworkServer.RegisterHandler(picked_up_sheep_message, SendPickedUpSheep);
        NetworkServer.RegisterHandler(dropped_sheep_message, SendDroppedSheep);
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
}
