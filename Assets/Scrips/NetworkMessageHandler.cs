using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public abstract class NetworkMessageHandler : NetworkBehaviour
{
    public const short player_movement_msg = 1000;
    public const short sheep_movement_msg = 1001;
    public const short picked_up_sheep_message = 1002;
    public const short dropped_sheep_message = 1003;
    public const short match_info_msg = 1004;

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

    public class PickedUpSheepMessage : MessageBase
    {
        public string playerName;
        public string sheepName;
        public Vector3 sheepPosition;
        public Quaternion sheepRotation;
        public float time;
        public int sheepState;
        public int sheepAnimation;
    }
    public class DroppedSheepMessage : MessageBase
    {
        public string playerName;
        public string sheepName;
        public int sheepState;
        public int sheepAnimation;
    }

    public class MatchInfoMessage : MessageBase
    {
        public int scoreTeamA;
        public int scoreTeamB;
        public int scoreTeamC;
        public float time;
    }
}
