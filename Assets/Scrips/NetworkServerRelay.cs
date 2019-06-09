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
        NetworkServer.RegisterHandler(player_movement_msg, OnReceivePlayerMovementMessage);
        NetworkServer.RegisterHandler(sheep_movement_msg, SendSheepMovement);
        NetworkServer.RegisterHandler(match_info_msg, SendmatchInfo);
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

    private void OnReceivePlayerMovementMessage(NetworkMessage _message)
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
        MatchInfoMessage _msg = _message.ReadMessage<MatchInfoMessage>();
        NetworkServer.SendToAll(match_info_msg, _msg);
    }
}
