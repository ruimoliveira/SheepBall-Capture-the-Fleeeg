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
    }

    public class MatchInfoMessage : MessageBase
    {
        public int scoreTeamA;
        public int scoreTeamB;
        public int scoreTeamC;
        public float time;
    }
}
