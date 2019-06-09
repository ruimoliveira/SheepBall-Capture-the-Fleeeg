using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public abstract class NetworkMessageHandler : NetworkBehaviour
{
    public const short player_movement_msg = 1000;
    public const short sheep_movement_msg = 1001;
    public const short match_info_msg = 1002;
    public const short join_team_msg = 1003;
    public const short lobby_info_msg = 1004;

    public class PlayerMovementMessage : MessageBase
    {
        public string objectTransformName;
        public Vector3 objectPosition;
        public Quaternion objectRotation;
        public float time;
    }

    public class SheepMovementMessage : MessageBase
    {
        public string objectTransformName;
        public Vector3 objectPosition;
        public Quaternion objectRotation;
        public float time;
        public int objectAnimation;
    }

    public class MatchInfoMessage : MessageBase
    {
        public int scoreTeamRed;
        public int scoreTeamBlue;
        public int scoreTeamYellow;
        public float time;
    }

    public class JoinTeamMessage : MessageBase
    {
        public string objectTransformName;
        public string nickname;
        public int team;
    }

    public class LobbyInfoMessage : MessageBase
    {
        public bool hasPlayer0;
        public string player0transformName;
        public string player0Nickname;

        public bool hasPlayer1;
        public string player1transformName;
        public string player1Nickname;

        public bool hasPlayer2;
        public string player2transformName;
        public string player2Nickname;

        public bool hasPlayer3;
        public string player3transformName;
        public string player3Nickname;

        public bool hasPlayer4;
        public string player4transformName;
        public string player4Nickname;

        public bool hasPlayer5;
        public string player5transformName;
        public string player5Nickname;

    }

}
